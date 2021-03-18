local sheet = {}
formulasheet.damage_buff = sheet
sheet.name = [[damage_buff]]
sheet.cellvalue = {}
sheet.cellformula = {}
sheet.cellabout = {}
sheet.totalrow = 228
for i = 1,sheet.totalrow do
	sheet.cellvalue[i] = {}
	sheet.cellformula[i] = {}
	sheet.cellabout[i] = {}
end

-- declares
sheet.input = {
param1 = 2--[[额外参数1（公式序号）]],
param2 = 3--[[额外参数2（攻击力系数）]],
param3 = 4--[[额外参数3（技能初始值）]],
param4 = 5--[[额外参数4（技能等级系数）]],
param5 = 6--[[额外参数5（群秒衰减系数）]],
param6 = 7--[[额外参数6（目标个数）]],
param7 = 8--[[额外参数7（BUFF添加概率）]],
param8 = 9--[[额外参数8（1-BUFF添加概率）]],
param9 = 10--[[额外参数9（BUFF-ID1）]],
param10 = 11--[[额外参数10（BUFF-ID2）]],
pattack = 14--[[物攻]],
mattack = 15--[[特攻]],
aplevel = 16--[[攻修]],
hit = 17--[[命中率]],
crit = 18--[[暴击率]],
alevel = 19--[[攻击者等级]],
evolveRadio = 20--[[进化技能系数]],
gradeRadio = 21--[[评级技能系数]],
ahpPrecent = 22--[[剩余血量比例]],
amaxHP = 23--[[最大血量]],
aspeed = 24--[[速度]],
aslevel = 25--[[Buff等级]],
pdefence = 28--[[物防]],
mdefence = 29--[[特防]],
dpplevel = 30--[[物防修]],
dmplevel = 31--[[特防修]],
dodge = 32--[[闪避率]],
tough = 33--[[韧性率]],
dlevel = 34--[[防御者等级]],
dhpPrecent = 35--[[剩余血量比例]],
dmaxHP = 36--[[最大血量]],
dspeed = 37--[[速度]]
}
sheet.output = {
damage1 = 2--[[计算结果]],
damage2 = 5--[[计算结果]],
damage3 = 8--[[计算结果]],
damage4 = 11--[[计算结果]],
damage5 = 14--[[计算结果]],
damage6 = 17--[[计算结果]],
damage7 = 20--[[计算结果]],
damage100 = 23--[[计算结果]],
damage101 = 26--[[计算结果]],
damage102 = 29--[[计算结果]],
damage50 = 32--[[计算结果]],
damage60 = 35--[[计算结果]],
damage70 = 38--[[计算结果]],
damage80 = 41--[[计算结果]],
damage90 = 44--[[计算结果]],
damage1001 = 47--[[计算结果]],
damage1002 = 50--[[计算结果]],
damage1003 = 53--[[计算结果]],
damage2001 = 56--[[计算结果]],
damage2002 = 59--[[计算结果]],
damage3003 = 62--[[计算结果]],
damage4001 = 65--[[计算结果]],
damage4002 = 68--[[计算结果]]
}

-- all datas
sheet.cellvalue[2][3] = 1
sheet.cellformula[2][6] = function(ins) return ins:get(7,8) end
sheet.cellvalue[3][3] = 0.0015
sheet.cellvalue[4][3] = 0.03
sheet.cellvalue[5][3] = 12.54
sheet.cellformula[5][6] = function(ins) return ins:get(8,8) end
sheet.cellvalue[6][3] = 3
sheet.cellvalue[7][3] = 1
sheet.cellformula[7][8] = function(ins) return (((ins:get(4,3) + (ins:get(5,3) * (ins:get(25,3) - 1))) * ins:get(20,3)) * ins:get(21,3)) end
sheet.cellvalue[8][3] = 0
sheet.cellformula[8][6] = function(ins) return ins:get(9,8) end
sheet.cellformula[8][8] = function(ins) return (0 - (((ins:get(4,3) + (ins:get(5,3) * (ins:get(25,3) - 1))) * ins:get(20,3)) * ins:get(21,3))) end
sheet.cellvalue[9][3] = 0
sheet.cellformula[9][8] = function(ins) return (0 - (((ins:get(4,3) + (ins:get(5,3) * (ins:get(25,3) - 1))) * ins:get(20,3)) * ins:get(21,3))) end
sheet.cellvalue[10][3] = 0
sheet.cellformula[10][8] = function(ins) return (((ins:get(4,3) + (ins:get(5,3) * (ins:get(25,3) - 1))) * ins:get(20,3)) * ins:get(21,3)) end
sheet.cellvalue[11][3] = 0
sheet.cellformula[11][6] = function(ins) return ins:get(10,8) end
sheet.cellformula[11][8] = function(ins) return (((ins:get(4,3) + (ins:get(5,3) * (ins:get(25,3) - 1))) * ins:get(20,3)) * ins:get(21,3)) end
sheet.cellformula[12][8] = function(ins) return (0 - (((ins:get(4,3) + (ins:get(5,3) * (ins:get(25,3) - 1))) * ins:get(20,3)) * ins:get(21,3))) end
sheet.cellformula[13][8] = function(ins) return (((ins:get(4,3) + (ins:get(5,3) * (ins:get(25,3) - 1))) * ins:get(20,3)) * ins:get(21,3)) end
sheet.cellvalue[14][3] = 600
sheet.cellformula[14][6] = function(ins) return ins:get(11,8) end
sheet.cellformula[14][8] = function(ins) return (ins:get(24,3) * ins:get(3,3)) end
sheet.cellvalue[15][3] = 580
sheet.cellformula[15][8] = function(ins) return (ins:get(23,3) * ins:get(3,3)) end
sheet.cellvalue[16][3] = 0
sheet.cellformula[16][8] = function(ins) return (0 - (ins:get(24,3) * ins:get(3,3))) end
sheet.cellvalue[17][3] = 0.1
sheet.cellformula[17][6] = function(ins) return ins:get(12,8) end
sheet.cellformula[17][8] = function(ins) return ((ins:get(15,3) * ins:get(3,3)) + ((ins:get(25,3) * ins:get(21,3)) * ins:get(4,3))) end
sheet.cellvalue[18][3] = 0.4
sheet.cellformula[18][8] = function(ins) return ((0.3 + (0.6 / ins:get(5,3))) * ((ins:get(15,3) * ins:get(3,3)) + ((ins:get(25,3) * ins:get(21,3)) * ins:get(4,3)))) end
sheet.cellvalue[19][3] = 20
sheet.cellformula[19][8] = function(ins) return (((((ins:get(15,3) * ins:get(3,3)) + ((ins:get(25,3) * ins:get(21,3)) * ins:get(4,3))) * ins:get(15,3)) / (ins:get(15,3) + (2 * ins:get(29,3)))) * 0.3) end
sheet.cellvalue[20][3] = 1
sheet.cellformula[20][6] = function(ins) return ins:get(11,8) end
sheet.cellformula[20][8] = function(ins) return ((((ins:get(14,3) * ins:get(3,3)) + (ins:get(21,3) * ins:get(4,3))) * ins:get(14,3)) / (ins:get(14,3) + (2 * ins:get(28,3)))) end
sheet.cellvalue[21][3] = 1
sheet.cellformula[21][8] = function(ins) return ((((ins:get(15,3) * ins:get(3,3)) + (ins:get(21,3) * ins:get(4,3))) * ins:get(15,3)) / (ins:get(15,3) + (2 * ins:get(29,3)))) end
sheet.cellvalue[22][3] = 0.6
sheet.cellformula[22][8] = function(ins) return (ins:get(25,3) * ins:get(3,3)) end
sheet.cellvalue[23][3] = 50000
sheet.cellformula[23][6] = function(ins) return ins:get(14,8) end
sheet.cellformula[23][8] = function(ins) return (((ins:get(25,3) * ins:get(3,3)) + 0.05) * ins:get(23,3)) end
sheet.cellvalue[24][3] = 100
sheet.cellformula[24][8] = function(ins) return (ins:get(3,3) + (ins:get(4,3) * ins:get(25,3))) end
sheet.cellvalue[25][3] = 20
sheet.cellformula[25][8] = function(ins) return (ins:get(3,3) * ins:get(25,3)) end
sheet.cellformula[26][6] = function(ins) return ins:get(15,8) end
sheet.cellformula[26][8] = function(ins) return ins:get(3,3) end
sheet.cellformula[27][8] = function(ins) return (ins:get(3,3) * ins:get(23,3)) end
sheet.cellvalue[28][3] = 900
sheet.cellformula[28][8] = function(ins) return (((((ins:get(15,3) * ins:get(3,3)) + (ins:get(25,3) * ins:get(4,3))) * 0.3) + (((ins:get(14,3) * ins:get(3,3)) + (ins:get(4,3) * ins:get(25,3))) * 0.3)) / 2) end
sheet.cellvalue[29][3] = 1000
sheet.cellformula[29][6] = function(ins) return ins:get(16,8) end
sheet.cellformula[29][8] = function(ins) return ((0.05 + (ins:get(3,3) * ins:get(25,3))) * ins:get(23,3)) end
sheet.cellvalue[30][3] = 0
sheet.cellformula[30][8] = function(ins) return (ins:get(4,3) + (ins:get(3,3) * ins:get(25,3))) end
sheet.cellvalue[31][3] = 0
sheet.cellvalue[32][3] = 0.05
sheet.cellformula[32][6] = function(ins) return ins:get(17,8) end
sheet.cellvalue[33][3] = 0.15
sheet.cellvalue[34][3] = 50
sheet.cellvalue[35][3] = 0.6
sheet.cellformula[35][6] = function(ins) return ins:get(18,8) end
sheet.cellvalue[36][3] = 1000
sheet.cellvalue[37][3] = 100
sheet.cellformula[38][6] = function(ins) return ins:get(19,8) end
sheet.cellformula[41][6] = function(ins) return ins:get(20,8) end
sheet.cellformula[44][6] = function(ins) return ins:get(21,8) end
sheet.cellformula[47][6] = function(ins) return ins:get(22,8) end
sheet.cellformula[50][6] = function(ins) return ins:get(23,8) end
sheet.cellformula[53][6] = function(ins) return ins:get(24,8) end
sheet.cellformula[56][6] = function(ins) return ins:get(25,8) end
sheet.cellformula[59][6] = function(ins) return ins:get(26,8) end
sheet.cellformula[62][6] = function(ins) return ins:get(27,8) end
sheet.cellformula[65][6] = function(ins) return ins:get(28,8) end
sheet.cellformula[68][6] = function(ins) return ins:get(29,8) end

-- cell data relation
sheet.cellabout[7][8] = {{2,6}}
sheet.cellabout[8][8] = {{5,6}}
sheet.cellabout[4][3] = {{7,8},{8,8},{9,8},{10,8},{11,8},{12,8},{13,8},{17,8},{18,8},{19,8},{20,8},{21,8},{24,8},{28,8},{30,8},{2,6},{5,6},{8,6},{11,6},{14,6},{20,6},{17,6},{32,6},{35,6},{38,6},{41,6},{44,6},{53,6},{65,6}}
sheet.cellabout[5][3] = {{7,8},{8,8},{9,8},{10,8},{11,8},{12,8},{13,8},{18,8},{2,6},{5,6},{8,6},{11,6},{14,6},{20,6},{17,6},{35,6}}
sheet.cellabout[25][3] = {{7,8},{8,8},{9,8},{10,8},{11,8},{12,8},{13,8},{17,8},{18,8},{19,8},{22,8},{23,8},{24,8},{25,8},{28,8},{29,8},{30,8},{2,6},{5,6},{8,6},{11,6},{14,6},{20,6},{17,6},{32,6},{35,6},{38,6},{47,6},{50,6},{53,6},{56,6},{65,6},{68,6}}
sheet.cellabout[20][3] = {{7,8},{8,8},{9,8},{10,8},{11,8},{12,8},{13,8},{2,6},{5,6},{8,6},{11,6},{14,6},{20,6},{17,6}}
sheet.cellabout[21][3] = {{7,8},{8,8},{9,8},{10,8},{11,8},{12,8},{13,8},{17,8},{18,8},{19,8},{20,8},{21,8},{2,6},{5,6},{8,6},{11,6},{14,6},{20,6},{17,6},{32,6},{35,6},{38,6},{41,6},{44,6}}
sheet.cellabout[9][8] = {{8,6}}
sheet.cellabout[10][8] = {{11,6}}
sheet.cellabout[11][8] = {{14,6},{20,6}}
sheet.cellabout[24][3] = {{14,8},{16,8},{23,6},{29,6}}
sheet.cellabout[3][3] = {{14,8},{15,8},{16,8},{17,8},{18,8},{19,8},{20,8},{21,8},{22,8},{23,8},{24,8},{25,8},{26,8},{27,8},{28,8},{29,8},{30,8},{23,6},{26,6},{29,6},{32,6},{35,6},{38,6},{41,6},{44,6},{47,6},{50,6},{53,6},{56,6},{59,6},{62,6},{65,6},{68,6}}
sheet.cellabout[23][3] = {{15,8},{23,8},{27,8},{29,8},{26,6},{50,6},{62,6},{68,6}}
sheet.cellabout[12][8] = {{17,6}}
sheet.cellabout[15][3] = {{17,8},{18,8},{19,8},{21,8},{28,8},{32,6},{35,6},{38,6},{44,6},{65,6}}
sheet.cellabout[29][3] = {{19,8},{21,8},{38,6},{44,6}}
sheet.cellabout[14][3] = {{20,8},{28,8},{41,6},{65,6}}
sheet.cellabout[28][3] = {{20,8},{41,6}}
sheet.cellabout[14][8] = {{23,6}}
sheet.cellabout[15][8] = {{26,6}}
sheet.cellabout[16][8] = {{29,6}}
sheet.cellabout[17][8] = {{32,6}}
sheet.cellabout[18][8] = {{35,6}}
sheet.cellabout[19][8] = {{38,6}}
sheet.cellabout[20][8] = {{41,6}}
sheet.cellabout[21][8] = {{44,6}}
sheet.cellabout[22][8] = {{47,6}}
sheet.cellabout[23][8] = {{50,6}}
sheet.cellabout[24][8] = {{53,6}}
sheet.cellabout[25][8] = {{56,6}}
sheet.cellabout[26][8] = {{59,6}}
sheet.cellabout[27][8] = {{62,6}}
sheet.cellabout[28][8] = {{65,6}}
sheet.cellabout[29][8] = {{68,6}}

-- enumerator
sheet.enumerator = {}
