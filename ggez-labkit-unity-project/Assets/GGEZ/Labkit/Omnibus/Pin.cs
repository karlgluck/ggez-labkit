// This is free and unencumbered software released into the public domain.
//
// Anyone is free to copy, modify, publish, use, compile, sell, or
// distribute this software, either in source code form or as a compiled
// binary, for any purpose, commercial or non-commercial, and by any
// means.
//
// In jurisdictions that recognize copyright laws, the author or authors
// of this software dedicate any and all copyright interest in the
// software to the public domain. We make this dedication for the benefit
// of the public at large and to the detriment of our heirs and
// successors. We intend this dedication to be an overt act of
// relinquishment in perpetuity of all present and future rights to this
// software under copyright law.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS BE LIABLE FOR ANY CLAIM, DAMAGES OR
// OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
// ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
//
// For more information, please refer to <http://unlicense.org/>


namespace GGEZ.Omnibus
{

public static partial class Pin
{
public const string INPUT = "IN";
public const string DATA = "DATA";
public const string SELECT = "SEL";
public const string ENABLE = "EN";
public const string CLOCK = "CLK";

public static bool IsInvalid (string pin)
    {
    return string.IsNullOrEmpty (pin);
    }

public static bool IsValid (string pin)
    {
    return !string.IsNullOrEmpty (pin);
    }


#region Adapter Pins
public static readonly string[] StdPin = StdPinAliases;
public static string[] StdPinAliases
    {
    get { return new string[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", }; }
    }
public const int StdPinCount = 26;
public const int Std_A_Index = 0;
public const int Std_B_Index = 1;
public const int Std_C_Index = 2;
public const int Std_D_Index = 3;
public const int Std_E_Index = 4;
public const int Std_F_Index = 5;
public const int Std_G_Index = 6;
public const int Std_H_Index = 7;
public const int Std_I_Index = 8;
public const int Std_J_Index = 9;
public const int Std_K_Index = 10;
public const int Std_L_Index = 11;
public const int Std_M_Index = 12;
public const int Std_N_Index = 13;
public const int Std_O_Index = 14;
public const int Std_P_Index = 15;
public const int Std_Q_Index = 16;
public const int Std_R_Index = 17;
public const int Std_S_Index = 18;
public const int Std_T_Index = 19;
public const int Std_U_Index = 20;
public const int Std_V_Index = 21;
public const int Std_W_Index = 22;
public const int Std_X_Index = 23;
public const int Std_Y_Index = 24;
public const int Std_Z_Index = 25;
#endregion



/*

A
B
C
D
E
F
G
H
I
J
K
L
M
N
O
P
Q
R
S
T
U
V
W
X
Y
Z

*/

}

}
