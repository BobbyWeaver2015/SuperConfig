using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace exporter
{
    public static partial class Exporter
    {
        static string FixFloat(string format)
        {
            Regex reg = new Regex("\\d+\\.\\d+(?!f)");
            return reg.Replace(format, match => match.Value + "f");
        }

        public static string TitleToUpper(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return string.Empty;

            char[] s = str.ToCharArray();
            char c = s[0];

            if ('a' <= c && c <= 'z')
                c = (char) (c & ~0x20);

            s[0] = c;

            return new string(s);
        }

        /// <summary>
        /// 声明CS
        /// </summary>
        static void AppendCSDeclara(ISheet sheet, int col, bool canWrite, StringBuilder sb)
        {
            Dictionary<string, List<int>> sameKeyDeclaras = new Dictionary<string, List<int>>();
            Regex regex = new Regex(@"(\d+)$", RegexOptions.Compiled | RegexOptions.CultureInvariant);

            for (int i = 0; i <= sheet.LastRowNum; i++)
            {
                IRow row = sheet.GetRow(i);
                if (row == null)
                    continue;
                ICell cell1 = row.GetCell(col, MissingCellPolicy.RETURN_NULL_AND_BLANK);
                ICell cell2 = row.GetCell(col + 1, MissingCellPolicy.RETURN_NULL_AND_BLANK);
                if (cell1 == null || cell2 == null || cell1.CellType == CellType.Blank ||
                    cell2.CellType == CellType.Blank)
                    continue;
                if (cell1.CellType != CellType.String || cell2.CellType != CellType.String)
                    throw new System.Exception("检查输入第" + (i + 1) + "行，sheetname=" + sheet.SheetName);
                string note = cell1.StringCellValue;
                string name = cell2.StringCellValue;
                if (string.IsNullOrEmpty(note) || string.IsNullOrEmpty(name) || name.StartsWith("_"))
                    continue;

                sb.Append("public float Get" + name.Substring(0, 1).ToUpper() + name.Substring(1) + "() { //" + note +
                          "\r\n");
                sb.Append("\treturn get(" + ((i + 1) * 1000 + col * 1 + 3) + ");\r\n");
                sb.Append("}\r\n");

                if (canWrite)
                {
                    sb.Append("public void Set" + name.Substring(0, 1).ToUpper() + name.Substring(1) + "(float v) {//" +
                              note + "\r\n");
                    sb.Append("\tset(" + ((i + 1) * 1000 + col + 3) + ",v);\r\n");
                    sb.Append("}\r\n");
                }

                var m = regex.Match(name);
                if (m.Success && name.Length > m.Value.Length)
                {
                    var key = name.Substring(0, name.Length - m.Value.Length);
                    if (!sameKeyDeclaras.ContainsKey(key))
                        sameKeyDeclaras.Add(key, new List<int>());
                    sameKeyDeclaras[key].Add(int.Parse(m.Value));
                }
            }

            foreach (var sd in sameKeyDeclaras)
            {
                if (sd.Value.Count < 2)
                    continue;

                var name = sd.Key.Substring(0, 1).ToUpper() + sd.Key.Substring(1);

                sb.Append("public float Get" + name + "(int key) {\r\n");
                sb.Append("switch (key){\r\n");
                foreach (var kv in sd.Value.OrderBy(kv => kv))
                    sb.Append("case " + kv + ": return Get" + name + kv + "();\r\n");
                sb.Append("}\r\n");
                sb.Append("return 0;\r\n");
                sb.Append("}\r\n");

                if (!canWrite)
                    continue;

                sb.Append("public void Set" + name + "(int key, float v) {\r\n");
                sb.Append("switch (key){\r\n");
                foreach (var kv in sd.Value.OrderBy(kv => kv))
                    sb.Append("case " + kv + ": Set" + name + kv + "(v); break;\r\n");
                sb.Append("}\r\n");
                sb.Append("}\r\n");
            }
        }

        static int CmpLen(List<object> l, List<object> r)
        {
            var sr = (string) r[1];

            var sl = (string) l[1];
            return sr.Length.CompareTo(sl.Length);
        }

        public static string DealWithFormulaSheetCS(ISheet sheet)
        {
            CodeTemplate.curlang = CodeTemplate.Langue.CS;
            StringBuilder sb = new StringBuilder();
            Dictionary<CellCoord, List<CellCoord>> abouts = new Dictionary<CellCoord, List<CellCoord>>();
            string sheetName = sheet.SheetName.Substring(3);
            string SheetName = sheetName.Substring(0, 1).ToUpper() + sheetName.Substring(1);
            string className = SheetName + "FormulaSheet";
            sb.Append("using System;\r\n");
            sb.Append("using System.Collections;\r\n");
            sb.Append("using System.Collections.Generic;\r\n");
            sb.Append("\r\n");

            // 扩展Config类统一获取某个表的实例对象
            sb.Append("public partial class Config {\r\n");
            sb.Append("\tpublic static " + className + " New" + className + "(){\r\n");
            sb.Append("\t\tvar formula = new " + className + "();\r\n");
            sb.Append("\t\tformula.Init();\r\n");
            sb.Append("\t\treturn formula;\r\n");
            sb.Append("\t}\r\n");
            sb.Append("}\r\n");

            //------

            // 开始生成这个配置表的算法类
            sb.Append("public class " + className + " : FormulaSheet { //定义数据表类开始\r\n");
            sb.Append("public void Init(){\r\n");

            // 数据内容
            for (int rownum = 0;
                rownum <= sheet.LastRowNum;
                rownum++)
            {
                IRow row = sheet.GetRow(rownum);
                if (row == null)
                    continue;

                for (int i = 0; i < row.Cells.Count; i++)
                {
                    ICell cell = row.Cells[i];
                    int colnum = cell.ColumnIndex;

                    // in、out声明忽略
                    if (colnum == 0 || colnum == 1 || colnum == 3 || colnum == 4)
                        continue;

                    if (cell.CellType == CellType.Boolean || cell.CellType == CellType.Numeric)
                    {
                        sb.Append("this.datas[" + ((rownum + 1) * 1000 + colnum + 1) + "] = " +
                                  (cell.CellType == CellType.Boolean
                                      ? (cell.BooleanCellValue ? 1 : 0).ToString()
                                      : cell.NumericCellValue.ToString()) + "f;\r\n");
                    }
                    else if (cell.CellType == CellType.Formula)
                    {
                        List<CellCoord> about;
                        sb.Append("this.funcs[" + ((rownum + 1) * 1000 + colnum + 1) + "] = ins => {\r\n");

                        string content = Formula2Code.Translate(sheet, cell.CellFormula, cell.ToString(), out about);
                        if (CodeTemplate.curlang == CodeTemplate.Langue.CS)
                        {
                            content = FixFloat(content);
                        }

                        sb.Append("\treturn (float)" + content + ";\r\n");
                        sb.Append("};\r\n");

                        CellCoord cur = new CellCoord(rownum + 1, colnum + 1);
                        foreach (CellCoord cc in about)
                        {
                            if (!abouts.ContainsKey(cc))
                                abouts.Add(cc, new List<CellCoord>());
                            if (!abouts[cc].Contains(cur))
                                abouts[cc].Add(cur);
                        }
                    }
                }
            }

            // 数据影响关联递归统计
            bool change;
            do

            {
                change = false;
                foreach (var item in abouts)
                {
                    for (int i = 0; i < item.Value.Count; i++)
                    {
                        if (abouts.ContainsKey(item.Value[i]))
                        {
                            foreach (var c in abouts[item.Value[i]])
                            {
                                if (!item.Value.Contains(c))
                                {
                                    item.Value.Add(c);
                                    change = true;
                                }
                            }
                        }
                    }
                }
            } while (change);

            // 数据影响关联
            foreach (var item in abouts)
                sb.Append("this.relation[" + (item.Key.row * 1000 + item.Key.col) + "] = new int[]{" +
                          string.Join(",", item.Value.Select(c => { return c.row * 1000 + c.col; })) + "};\r\n");
            sb.Append("} // 初始化数据结束\r\n");

            // 声明
            AppendCSDeclara(sheet, 0, true, sb);
            AppendCSDeclara(sheet, 3, false, sb);

            // 枚举器
            foreach (var item in FormulaEnumerator.GetList(sheet))
            {
                // 写结构
                sb.Append("public struct " + item.fullName + " {\r\n");

                // 属性
                sb.Append("\tpublic " + className + " sheet;\r\n");
                sb.Append("\t int line;\r\n");
                for (int i = 0;
                    i < item.propertys.Count;
                    i++)
                    sb.Append("\tfloat " + item.propertys[i] + "; // " + item.notes[i] + "\r\n");

                // 枚举方法
                sb.Append("public bool MoveNext() {\r\n");

                // MoveNext
                sb.Append("\tif (line <= 0) {\r\n");
                sb.Append("\t\tline = " + (item.start + 1) * 1000 + ";\r\n");
                sb.Append("\t} else {\r\n");
                sb.Append("\t\tline = line + " + item.div * 1000 + ";\r\n");
                sb.Append("}\r\n");
                sb.Append("\tif (line >= " + (item.end + 1) * 1000 + ") {\r\n");
                sb.Append("\t\treturn false;\r\n");
                sb.Append("}\r\n");
                sb.Append("\tif (sheet.get(line+" + (6 + 1000 * (item.key - 1)) + ") == 0 ) {\r\n");
                sb.Append("\t\treturn MoveNext();\r\n");
                sb.Append("}\r\n");
                for (int i = 0;
                    i < item.propertys.Count;
                    i++)
                    sb.Append("" + item.propertys[i] + " = sheet.get(line+" + (6 + 1000 * i) + ");\r\n");
                sb.Append("\treturn true;\r\n");
                sb.Append("} // 枚举方法next结束\r\n");
                sb.Append("} // 枚举struct定义结束\r\n");

                // GetEnumerator
                sb.Append("public " + item.fullName + " Get" + item.name + "Enumerator(){\r\n");
                sb.Append("\tvar enumerator = new " + item.fullName + "();\r\n");
                sb.Append("\t\tenumerator.sheet = this;\r\n");
                sb.Append("\t\treturn enumerator;\r\n");
                sb.Append("}\r\n");
            }

            sb.Append("}\r\n");

            // 结果
            formulaContents.Add(SheetName, sb.ToString());
            return string.Empty;
        }

        public static string ExportCS(string codeExportDir, string configExportDir)
        {
// 目录清理
            if (Directory.Exists(configExportDir))
                new DirectoryInfo(configExportDir).GetFiles().Where(f => f.Extension == ".json")
                    .ToList().ForEach(fi => { fi.Delete(); });
            else
                Directory.CreateDirectory(configExportDir);
            if (!Directory.Exists(codeExportDir)) Directory.CreateDirectory(codeExportDir);
            new DirectoryInfo(codeExportDir).GetFiles("data_*.cs").ToList<FileInfo>().ForEach(fi => { fi.Delete(); });
            new DirectoryInfo(codeExportDir).GetFiles("formula_*.cs").ToList<FileInfo>()
                .ForEach(fi => { fi.Delete(); });

// stream加载的接口
            Dictionary<string, string> typestream_read = new Dictionary<string, string>();
            typestream_read.Add("int", "ReadInt32");
            typestream_read.Add("string", "ReadString");
            typestream_read.Add("double", "ReadDouble");
            typestream_read.Add("float", "ReadSingle");
            typestream_read.Add("int32", "ReadInt32");
            typestream_read.Add("long", "ReadInt64");

// 类型转换
            Dictionary<string, string> typeconvert = new Dictionary<string, string>();
            typeconvert.Add("int", "int");
            typeconvert.Add("int32", "int");
            typeconvert.Add("long", "long");
            typeconvert.Add("string", "string");
            typeconvert.Add("float", "float");
            typeconvert.Add("double", "double");
            typeconvert.Add("[]int", "int[]");
            typeconvert.Add("[]int32", "int[]");
            typeconvert.Add("[]long", "long[]");
            typeconvert.Add("[]string", "string[]");
            typeconvert.Add("[]double", "double[]");
            typeconvert.Add("[]float", "float[]");

// 索引类型转换
            Dictionary<string, string> mapTypeConvert = new Dictionary<string, string>();
            mapTypeConvert.Add("int", "int");
            mapTypeConvert.Add("float", "float");
            mapTypeConvert.Add("int32", "int");
            mapTypeConvert.Add("string", "string");
            mapTypeConvert.Add("float32", "float");
            mapTypeConvert.Add("double", "double");
            int goWriteCount = 0;
            List<string> loadfuncs = new List<string>();
            List<string> clearfuncs = new List<string>();
            List<string> savefuncs = new List<string>();

// 写公式
            foreach (var formula in formulaContents)
            {
                File.WriteAllText(codeExportDir + "formula_" + formula.Key.ToLower() + ".cs", formula.Value,
                    new UTF8Encoding(false));
                Interlocked.Increment(ref goWriteCount);
// lock (loadfuncs) loadfuncs.Add("loadFormula" + formula.Key);
            }

            List<string> results = new List<string>();

// 写cs
            foreach (var data in datas.Values)
            {
                ThreadPool.QueueUserWorkItem(ooo =>
                {
	                string str = data.name.Substring(0, 1).ToUpper() + data.name.Substring(1);
	                string text = str + "Table";
	                string text2 = str + "Config";
	                string text3 = str + "TableGroup";
	                StringBuilder stringBuilder2 = new StringBuilder();
	                stringBuilder2.Append("using System;\r\n");
	                stringBuilder2.Append("using UnityEngine;\r\n");
	                stringBuilder2.Append("using System.Collections;\r\n");
	                stringBuilder2.Append("using System.Collections.Generic;\r\n");
	                stringBuilder2.Append("using Model.Load;\r\n");
	                stringBuilder2.Append("\r\n");
	                stringBuilder2.Append("public partial class Config {\r\n");
	                stringBuilder2.Append(string.Concat(new string[]
	                {
		                "\tstatic ",
		                text,
		                " _",
		                text,
		                ";\r\n"
	                }));
	                stringBuilder2.Append(string.Concat(new string[]
	                {
		                "\tpublic static ",
		                text,
		                " Get",
		                text,
		                "(){\r\n"
	                }));
	                stringBuilder2.Append(string.Format("\tif({0} == null) Load{1}();\r\n", "_" + text, text));
	                stringBuilder2.Append("\t\treturn _" + text + ";\r\n");
	                stringBuilder2.Append("\t}\r\n");
	                stringBuilder2.Append("\tpublic static void Load" + text + "(){\r\n");
	                stringBuilder2.Append(string.Format(
		                "\t\tvar json = SuperLoader.LoadConfig(\"{0}\").text;\r\n", data.name));
	                stringBuilder2.Append(string.Format("\t\t{0} = LitJson.JsonMapper.ToObject<{1}>(json);\r\n",
		                "_" + text, text));
	                stringBuilder2.Append("\t}\r\n");
	                stringBuilder2.Append(string.Format("\tpublic static void Clear{0} () {{\r\n", text));
	                stringBuilder2.Append(string.Format("\t\t_{0} = null;\r\n", text));
	                stringBuilder2.Append("\t}\r\n");
	                lock (clearfuncs)
	                {
		                clearfuncs.Add("Config.Clear" + text);
	                }

	                stringBuilder2.Append("}\r\n");
	                stringBuilder2.Append("public class " + text3 + " {\r\n");
	                foreach (KeyValuePair<string, string[]> current4 in data.groups)
	                {
		                stringBuilder2.Append("\tpublic ");
		                string[] value = current4.Value;
		                for (int i = 0; i < value.Length; i++)
		                {
			                string text4 = value[i];
			                stringBuilder2.Append("Dictionary<string,");
		                }

		                stringBuilder2.Append("int[]");
		                string[] value2 = current4.Value;
		                for (int j = 0; j < value2.Length; j++)
		                {
			                string text5 = value2[j];
			                stringBuilder2.Append(">");
		                }

		                stringBuilder2.Append(" ");
		                string str2 = current4.Key.Substring(0, 1).ToUpper() +
		                              current4.Key.Replace("|", "_").Substring(1);
		                stringBuilder2.Append(str2 + ";\r\n");
	                }

	                stringBuilder2.Append("}\r\n");
	                stringBuilder2.Append("public class " + text2 + " {\r\n");
	                for (int k = 0; k < data.keys.Count; k++)
	                {
		                stringBuilder2.Append(string.Concat(new string[]
		                {
			                "\tpublic ",
			                typeconvert[data.types[k]],
			                " ",
			                data.keys[k].Substring(0, 1).ToUpper(),
			                data.keys[k].Substring(1),
			                "; // ",
			                data.keyNames[k],
			                "\r\n"
		                }));
	                }

	                stringBuilder2.Append("}\r\n");
	                stringBuilder2.Append("// " + string.Join(",", data.files) + "\r\n");
	                stringBuilder2.Append("public class " + text + " {\r\n");
	                stringBuilder2.Append("\tpublic string Name;\r\n");
	                stringBuilder2.Append(string.Format("\tpublic Dictionary<string, {0}> _Datas;\r\n", text2));
	                stringBuilder2.Append(string.Format("\tpublic {0} _Group;\r\n", text3));
	                foreach (KeyValuePair<string, string[]> current5 in data.groups)
	                {
		                string str3 = current5.Key.Substring(0, 1).ToUpper() +
		                              current5.Key.Replace("|", "_").Substring(1);
		                stringBuilder2.Append("\tprivate ");
		                stringBuilder2.Append("Dictionary<string,");
		                stringBuilder2.Append(text2 + "[]> " + str3 + "_Cached = new ");
		                stringBuilder2.Append("Dictionary<string," + text2 + "[]>();\r\n");
	                }

	                stringBuilder2.Append("public " + text2 + " Get(int id) {\r\n");
	                stringBuilder2.Append("\tstring k = id.ToString();\r\n");
	                stringBuilder2.Append("\t" + text2 + " ret;\r\n");
	                stringBuilder2.Append("\tif (_Datas.TryGetValue(k,out ret))\r\n");
	                stringBuilder2.Append("\t\treturn ret;\r\n");
	                stringBuilder2.Append("\treturn null;\r\n");
	                stringBuilder2.Append("}\r\n");
	                foreach (KeyValuePair<string, string[]> current6 in data.groups)
	                {
		                string text6 = current6.Key.Substring(0, 1).ToUpper() +
		                               current6.Key.Replace("|", "_").Substring(1);
		                stringBuilder2.Append(string.Concat(new string[]
		                {
			                "\tpublic ",
			                text2,
			                "[] Get_",
			                current6.Key.Replace("|", "_"),
			                "("
		                }));
		                string[] value3 = current6.Value;
		                for (int l = 0; l < value3.Length; l++)
		                {
			                string text7 = value3[l];
			                stringBuilder2.Append(string.Concat(new string[]
			                {
				                mapTypeConvert[typeconvert[data.types[data.keys.IndexOf(text7)]]],
				                " ",
				                text7.Substring(0, 1).ToUpper(),
				                text7.Substring(1),
				                ","
			                }));
		                }

		                stringBuilder2.Remove(stringBuilder2.Length - 1, 1);
		                stringBuilder2.Append(") {\r\n");
		                string text8 = "string.Empty";
		                string[] value4 = current6.Value;
		                for (int m = 0; m < value4.Length; m++)
		                {
			                string text9 = value4[m];
			                string str4 = text9.Substring(0, 1).ToUpper() + text9.Substring(1);
			                text8 = text8 + "+" + str4 + "+\"_\"";
		                }

		                string str5 = text6 + "_Cached";
		                stringBuilder2.Append("string cach_key = " + text8 + ";\r\n");
		                stringBuilder2.Append(text2 + "[] ret;\r\n");
		                stringBuilder2.Append("if(" + str5 + ".TryGetValue(cach_key,out ret))\r\n");
		                stringBuilder2.Append("\treturn ret;\r\n");
		                string text10 = "_Group." + text6;
		                string text11 = "";
		                for (int n = 0; n < current6.Value.Length; n++)
		                {
			                bool flag4 = n == 0;
			                if (flag4)
			                {
				                stringBuilder2.Append("if (" + text10 + ".ContainsKey(");
				                text11 = current6.Value[n].Substring(0, 1).ToUpper() + current6.Value[n].Substring(1);
				                stringBuilder2.Append(text11 + ".ToString()) ){\r\n");
			                }
			                else
			                {
				                string text12 = "tmp" + (n - 1);
				                stringBuilder2.Append(string.Concat(new string[]
				                {
					                "var ",
					                text12,
					                " = ",
					                text10,
					                "[",
					                text11,
					                ".ToString()];\r\n"
				                }));
				                stringBuilder2.Append("if (" + text12 + ".ContainsKey(");
				                text10 = text12;
				                text11 = current6.Value[n].Substring(0, 1).ToUpper() + current6.Value[n].Substring(1);
				                stringBuilder2.Append(text11 + ".ToString()) ){\r\n");
			                }
		                }

		                stringBuilder2.Append(string.Concat(new string[]
		                {
			                "var ids = ",
			                text10,
			                "[",
			                text11,
			                ".ToString()];\r\n"
		                }));
		                stringBuilder2.Append("var configs = new " + text2 + "[ids.Length];\r\n");
		                stringBuilder2.Append("for (int i = 0; i < ids.Length; i++) {\r\n");
		                stringBuilder2.Append("\tvar id = ids[i];\r\n");
		                stringBuilder2.Append("\tconfigs[i] = Get(id);\r\n");
		                stringBuilder2.Append("}\r\n");
		                stringBuilder2.Append("\t" + str5 + "[cach_key]=configs;\r\n");
		                stringBuilder2.Append("return configs;\r\n");
		                for (int num = 0; num < current6.Value.Length; num++)
		                {
			                stringBuilder2.Append("}\r\n");
		                }

		                stringBuilder2.Append("return new " + text2 + "[0];\r\n");
		                stringBuilder2.Append("}\r\n");
	                }

	                for (int num2 = 0; num2 < data.keys.Count; num2++)
	                {
		                bool flag5 = data.types[num2] == "string" || data.types[num2].StartsWith("[]");
		                if (!flag5)
		                {
			                stringBuilder2.Append(string.Concat(new object[]
			                {
				                "\tpublic float data_",
				                data.name,
				                "_vlookup_",
				                data.cols[num2] + 1,
				                "(int id) {\r\n"
			                }));
			                stringBuilder2.Append(string.Concat(new string[]
			                {
				                "\treturn (float)(Config.Get",
				                text,
				                "()._Datas[id.ToString()].",
				                data.keys[num2].Substring(0, 1).ToUpper(),
				                data.keys[num2].Substring(1),
				                ");\r\n"
			                }));
			                stringBuilder2.Append("}\r\n");
		                }
	                }

	                stringBuilder2.Append("}\r\n");
	                File.WriteAllText(codeExportDir + "data_" + data.name + ".cs", stringBuilder2.ToString());
	                Interlocked.Increment(ref goWriteCount);
	                lock (loadfuncs)
	                {
		                loadfuncs.Add("Config.Load" + text);
	                }

	                lock (results)
	                {
		                results.Add(string.Empty);
	                }
                });
            }

// 写json
            foreach (var data in datas.Values)
            {
                ThreadPool.QueueUserWorkItem(ooo =>
                {
                    JObject config = new JObject();
                    config["Name"] = data.name;
                    config["Crc32"] = data.crc32;

                    JObject datas = new JObject();
                    config["_Datas"] = datas;
                    foreach (var line in data.dataContent)
                    {
                        JObject ll = new JObject();
                        for (int j = 0; j < data.keys.Count; j++)
                            ll[data.keys[j].TitleToUpper()] = JToken.FromObject(line[j]);
                        datas[line[0].ToString()] = ll;
                    }

                    JObject group = new JObject();
                    config["_Group"] = group;
                    Dictionary<string, string[]>.Enumerator enumerator = data.groups.GetEnumerator();
                    while (enumerator.MoveNext())
                        group[enumerator.Current.Key.Replace("|", "_").TitleToUpper()] = new JObject();
                    foreach (var values in data.dataContent)
                    {
                        enumerator = data.groups.GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            JObject cur = group[enumerator.Current.Key.Replace("|", "_").TitleToUpper()] as JObject;
                            string key = string.Empty;
                            for (int j = 0; j < enumerator.Current.Value.Length - 1; j++)
                            {
                                key = values[data.groupindexs[enumerator.Current.Key][j]].ToString();
                                if (cur[key] == null)
                                    cur[key] = new JObject();
                                cur = cur[key] as JObject;
                            }

                            key = values[data.groupindexs[enumerator.Current.Key][enumerator.Current.Value.Length - 1]]
                                .ToString();
                            if (cur[key] == null)
                                cur[key] = new JArray();
                            (cur[key] as JArray).Add(JToken.FromObject(values[0]));
                        }
                    }

                    StringWriter textWriter = new StringWriter();
                    textWriter.NewLine = "\r\n";
                    JsonTextWriter jsonWriter = new JsonTextWriter(textWriter)
                    {
                        Formatting = Formatting.Indented,
                        Indentation = 4,
                        IndentChar = ' '
                    };
                    new JsonSerializer().Serialize(jsonWriter, config);
                    var content = textWriter.ToString();
                    File.WriteAllText(configExportDir + data.name + ".json", content, new UTF8Encoding(false));

                    lock (results)
                        results.Add(string.Empty);
                });
            }

// 格式化代码
            while (goWriteCount < formulaContents.Count + datas.Values.Count)
                Thread.Sleep(10);

// 写加载
            loadfuncs.Sort();
            StringBuilder loadcode = new StringBuilder();
            loadcode.Append("using System;\r\n");
            loadcode.Append("using System.Collections;\r\n");
            loadcode.Append("using System.Collections.Generic;\r\n");
            loadcode.Append("\r\n");

// 扩展Config类统一获取某个表的实例对象
            loadcode.Append("public partial class Config {\r\n");

// load all
            loadcode.Append("\tpublic static void Load() {\r\n");
            foreach (var str in loadfuncs)
                loadcode.Append("\t" + str + "();\r\n");
            loadcode.Append("}\r\n");

// clear all
            clearfuncs.Sort();
            loadcode.Append("\tpublic static void Clear() {\r\n");
            foreach (var str in clearfuncs)
                loadcode.Append("\t" + str + "();\r\n");
            loadcode.Append("}\r\n");

// save all
            savefuncs.Sort();
            loadcode.Append("\tpublic static void Save() {\r\n");
            foreach (var str in savefuncs)
                loadcode.Append("\t" + str + "();\r\n");
            loadcode.Append("}\r\n");
            loadcode.Append("}\r\n");
            File.WriteAllText(codeExportDir + "load.cs", loadcode.ToString());

// 等待所有文件完成
            while (results.Count < datas.Values.Count * 2)
                Thread.Sleep(TimeSpan.FromSeconds(0.01));
            return string.Empty;
        }
    }
}