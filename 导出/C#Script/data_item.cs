using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Model.Load;

public partial class Config {
	static ItemTable _ItemTable;
	public static ItemTable GetItemTable(){
	if(_ItemTable == null) LoadItemTable();
		return _ItemTable;
	}
	public static void LoadItemTable(){
		var json = SuperLoader.LoadConfig("item").text;
		_ItemTable = LitJson.JsonMapper.ToObject<ItemTable>(json);
	}
	public static void ClearItemTable () {
		_ItemTable = null;
	}
}
public class ItemTableGroup {
	public Dictionary<string,int[]> Type;
	public Dictionary<string,Dictionary<string,int[]>> Type_SubType;
	public Dictionary<string,Dictionary<string,Dictionary<string,int[]>>> Type_SubType_Param1;
	public Dictionary<string,Dictionary<string,Dictionary<string,Dictionary<string,int[]>>>> Type_SubType_Param1_Param2;
}
public class ItemConfig {
	public int Id; // 道具ID
	public string Name; // 道具名
	public int Type; // 道具类型
	public int SubType; // 道具子类型
	public string Icon; // 道具图标
	public int Quality; // 品质
	public int UseLevel; // 使用等级
	public int Price; // 售价
	public int MaxCount; // 最大堆叠数
	public string Obtain; // 获得途径
	public string Desc; // 描述
	public int Param1; // 影响值1
	public int Param2; // 影响值2
	public int Param3; // 影响值3
	public string Param4; // 影响值4
}
// 道具表.xlsx
public class ItemTable {
	public string Name;
	public Dictionary<string, ItemConfig> _Datas;
	public ItemTableGroup _Group;
	private Dictionary<string,ItemConfig[]> Type_Cached = new Dictionary<string,ItemConfig[]>();
	private Dictionary<string,ItemConfig[]> Type_SubType_Cached = new Dictionary<string,ItemConfig[]>();
	private Dictionary<string,ItemConfig[]> Type_SubType_Param1_Cached = new Dictionary<string,ItemConfig[]>();
	private Dictionary<string,ItemConfig[]> Type_SubType_Param1_Param2_Cached = new Dictionary<string,ItemConfig[]>();
public ItemConfig Get(int id) {
	string k = id.ToString();
	ItemConfig ret;
	if (_Datas.TryGetValue(k,out ret))
		return ret;
	return null;
}
	public ItemConfig[] Get_Type(int Type) {
string cach_key = string.Empty+Type+"_";
ItemConfig[] ret;
if(Type_Cached.TryGetValue(cach_key,out ret))
	return ret;
if (_Group.Type.ContainsKey(Type.ToString()) ){
var ids = _Group.Type[Type.ToString()];
var configs = new ItemConfig[ids.Length];
for (int i = 0; i < ids.Length; i++) {
	var id = ids[i];
	configs[i] = Get(id);
}
	Type_Cached[cach_key]=configs;
return configs;
}
return new ItemConfig[0];
}
	public ItemConfig[] Get_Type_SubType(int Type,int SubType) {
string cach_key = string.Empty+Type+"_"+SubType+"_";
ItemConfig[] ret;
if(Type_SubType_Cached.TryGetValue(cach_key,out ret))
	return ret;
if (_Group.Type_SubType.ContainsKey(Type.ToString()) ){
var tmp0 = _Group.Type_SubType[Type.ToString()];
if (tmp0.ContainsKey(SubType.ToString()) ){
var ids = tmp0[SubType.ToString()];
var configs = new ItemConfig[ids.Length];
for (int i = 0; i < ids.Length; i++) {
	var id = ids[i];
	configs[i] = Get(id);
}
	Type_SubType_Cached[cach_key]=configs;
return configs;
}
}
return new ItemConfig[0];
}
	public ItemConfig[] Get_Type_SubType_Param1(int Type,int SubType,int Param1) {
string cach_key = string.Empty+Type+"_"+SubType+"_"+Param1+"_";
ItemConfig[] ret;
if(Type_SubType_Param1_Cached.TryGetValue(cach_key,out ret))
	return ret;
if (_Group.Type_SubType_Param1.ContainsKey(Type.ToString()) ){
var tmp0 = _Group.Type_SubType_Param1[Type.ToString()];
if (tmp0.ContainsKey(SubType.ToString()) ){
var tmp1 = tmp0[SubType.ToString()];
if (tmp1.ContainsKey(Param1.ToString()) ){
var ids = tmp1[Param1.ToString()];
var configs = new ItemConfig[ids.Length];
for (int i = 0; i < ids.Length; i++) {
	var id = ids[i];
	configs[i] = Get(id);
}
	Type_SubType_Param1_Cached[cach_key]=configs;
return configs;
}
}
}
return new ItemConfig[0];
}
	public ItemConfig[] Get_Type_SubType_Param1_Param2(int Type,int SubType,int Param1,int Param2) {
string cach_key = string.Empty+Type+"_"+SubType+"_"+Param1+"_"+Param2+"_";
ItemConfig[] ret;
if(Type_SubType_Param1_Param2_Cached.TryGetValue(cach_key,out ret))
	return ret;
if (_Group.Type_SubType_Param1_Param2.ContainsKey(Type.ToString()) ){
var tmp0 = _Group.Type_SubType_Param1_Param2[Type.ToString()];
if (tmp0.ContainsKey(SubType.ToString()) ){
var tmp1 = tmp0[SubType.ToString()];
if (tmp1.ContainsKey(Param1.ToString()) ){
var tmp2 = tmp1[Param1.ToString()];
if (tmp2.ContainsKey(Param2.ToString()) ){
var ids = tmp2[Param2.ToString()];
var configs = new ItemConfig[ids.Length];
for (int i = 0; i < ids.Length; i++) {
	var id = ids[i];
	configs[i] = Get(id);
}
	Type_SubType_Param1_Param2_Cached[cach_key]=configs;
return configs;
}
}
}
}
return new ItemConfig[0];
}
	public float data_item_vlookup_1(int id) {
	return (float)(Config.GetItemTable()._Datas[id.ToString()].Id);
}
	public float data_item_vlookup_3(int id) {
	return (float)(Config.GetItemTable()._Datas[id.ToString()].Type);
}
	public float data_item_vlookup_4(int id) {
	return (float)(Config.GetItemTable()._Datas[id.ToString()].SubType);
}
	public float data_item_vlookup_6(int id) {
	return (float)(Config.GetItemTable()._Datas[id.ToString()].Quality);
}
	public float data_item_vlookup_7(int id) {
	return (float)(Config.GetItemTable()._Datas[id.ToString()].UseLevel);
}
	public float data_item_vlookup_8(int id) {
	return (float)(Config.GetItemTable()._Datas[id.ToString()].Price);
}
	public float data_item_vlookup_9(int id) {
	return (float)(Config.GetItemTable()._Datas[id.ToString()].MaxCount);
}
	public float data_item_vlookup_12(int id) {
	return (float)(Config.GetItemTable()._Datas[id.ToString()].Param1);
}
	public float data_item_vlookup_13(int id) {
	return (float)(Config.GetItemTable()._Datas[id.ToString()].Param2);
}
	public float data_item_vlookup_14(int id) {
	return (float)(Config.GetItemTable()._Datas[id.ToString()].Param3);
}
}
