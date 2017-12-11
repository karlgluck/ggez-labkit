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

using UnityEngine;

namespace GGEZ
{

public static partial class KeyCodeExt
{
public static bool ToChar (
        this KeyCode self,
        bool shift,
        out char charValue
        )
    {
    charValue = '\0';
    switch (self)
        {
        case KeyCode.Tab: charValue = '\t'; break;
        case KeyCode.Return: charValue = '\n'; break;
        case KeyCode.Space: charValue = ' '; break;
        case KeyCode.Exclaim: charValue = '!'; break;
        case KeyCode.DoubleQuote: charValue = '"'; break;
        case KeyCode.Hash: charValue = '#'; break;
        case KeyCode.Dollar: charValue = '$'; break;
        case KeyCode.Ampersand: charValue = '%'; break;
        case KeyCode.Quote: charValue = '\''; break;
        case KeyCode.LeftParen: charValue = '('; break;
        case KeyCode.RightParen: charValue = ')'; break;
        case KeyCode.Asterisk: charValue = '*'; break;
        case KeyCode.Plus: charValue = '+'; break;
        case KeyCode.Comma: charValue = ','; break;
        case KeyCode.Minus: charValue = '-'; break;
        case KeyCode.Period: charValue = '.'; break;
        case KeyCode.Slash: charValue = '/'; break;
        case KeyCode.Alpha0: charValue = shift ? ')' : '0'; break;
        case KeyCode.Alpha1: charValue = shift ? '!' : '1'; break;
        case KeyCode.Alpha2: charValue = shift ? '@' : '2'; break;
        case KeyCode.Alpha3: charValue = shift ? '#' : '3'; break;
        case KeyCode.Alpha4: charValue = shift ? '$' : '4'; break;
        case KeyCode.Alpha5: charValue = shift ? '%' : '5'; break;
        case KeyCode.Alpha6: charValue = shift ? '^' : '6'; break;
        case KeyCode.Alpha7: charValue = shift ? '&' : '7'; break;
        case KeyCode.Alpha8: charValue = shift ? '*' : '8'; break;
        case KeyCode.Alpha9: charValue = shift ? '(' : '9'; break;
        case KeyCode.Colon: charValue = shift ? ':' : ';'; break;
        case KeyCode.Semicolon: charValue = ';'; break;
        case KeyCode.Less: charValue = '<'; break;
        case KeyCode.Equals: charValue = '='; break;
        case KeyCode.Greater: charValue = '>'; break;
        case KeyCode.Question: charValue = '?'; break;
        case KeyCode.At: charValue = '@'; break;
        case KeyCode.LeftBracket: charValue = '['; break;
        case KeyCode.Backslash: charValue = '\\'; break;
        case KeyCode.RightBracket: charValue = ']'; break;
        case KeyCode.Caret: charValue = '^'; break;
        case KeyCode.Underscore: charValue = '_'; break;
        case KeyCode.BackQuote: charValue = '`'; break;
        case KeyCode.A: charValue = shift ? 'A' : 'a'; break;
        case KeyCode.B: charValue = shift ? 'B' : 'b'; break;
        case KeyCode.C: charValue = shift ? 'C' : 'c'; break;
        case KeyCode.D: charValue = shift ? 'D' : 'd'; break;
        case KeyCode.E: charValue = shift ? 'E' : 'e'; break;
        case KeyCode.F: charValue = shift ? 'F' : 'f'; break;
        case KeyCode.G: charValue = shift ? 'G' : 'g'; break;
        case KeyCode.H: charValue = shift ? 'H' : 'h'; break;
        case KeyCode.I: charValue = shift ? 'I' : 'i'; break;
        case KeyCode.J: charValue = shift ? 'J' : 'j'; break;
        case KeyCode.K: charValue = shift ? 'K' : 'k'; break;
        case KeyCode.L: charValue = shift ? 'L' : 'l'; break;
        case KeyCode.M: charValue = shift ? 'M' : 'm'; break;
        case KeyCode.N: charValue = shift ? 'N' : 'n'; break;
        case KeyCode.O: charValue = shift ? 'O' : 'o'; break;
        case KeyCode.P: charValue = shift ? 'P' : 'p'; break;
        case KeyCode.Q: charValue = shift ? 'Q' : 'q'; break;
        case KeyCode.R: charValue = shift ? 'R' : 'r'; break;
        case KeyCode.S: charValue = shift ? 'S' : 's'; break;
        case KeyCode.T: charValue = shift ? 'T' : 't'; break;
        case KeyCode.U: charValue = shift ? 'U' : 'u'; break;
        case KeyCode.V: charValue = shift ? 'V' : 'v'; break;
        case KeyCode.W: charValue = shift ? 'W' : 'w'; break;
        case KeyCode.X: charValue = shift ? 'X' : 'x'; break;
        case KeyCode.Y: charValue = shift ? 'Y' : 'y'; break;
        case KeyCode.Z: charValue = shift ? 'Z' : 'z'; break;
        case KeyCode.Keypad0: charValue = '0'; break;
        case KeyCode.Keypad1: charValue = '1'; break;
        case KeyCode.Keypad2: charValue = '2'; break;
        case KeyCode.Keypad3: charValue = '3'; break;
        case KeyCode.Keypad4: charValue = '4'; break;
        case KeyCode.Keypad5: charValue = '5'; break;
        case KeyCode.Keypad6: charValue = '6'; break;
        case KeyCode.Keypad7: charValue = '7'; break;
        case KeyCode.Keypad8: charValue = '8'; break;
        case KeyCode.Keypad9: charValue = '9'; break;
        case KeyCode.KeypadPeriod: charValue = '.'; break;
        case KeyCode.KeypadDivide: charValue = '/'; break;
        case KeyCode.KeypadMultiply: charValue = '*'; break;
        case KeyCode.KeypadMinus: charValue = '-'; break;
        case KeyCode.KeypadPlus: charValue = '+'; break;
        case KeyCode.KeypadEnter: charValue = '\n'; break;
        case KeyCode.KeypadEquals: charValue = '='; break;
        }
    return charValue != '\0';
    }
}




}