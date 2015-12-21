#region GNU license
// MyFilms - Plugin for Mediaportal
// http://www.team-mediaportal.com
// Copyright (C) 2006-2007
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
//
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//
//+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
//+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
#endregion

namespace MyFilmsPlugin.MyFilms.Utils
{
  using System;
  using System.IO;
  using System.Text;
  using System.Security.Cryptography;

  public class Crypto
  {
    readonly byte[] clef = { 0xAD, 0x24, 0xFE, 0x58, 0xC5, 0x81, 0x37, 0xB4, 0xF9, 0x97, 0x23, 0xD2, 0x13, 0x86, 0xBB, 0xA7 };
    readonly byte[] vect = { 0x81, 0xFD, 0xC3, 0xBB, 0x0A, 0xE6, 0xFE, 0xB8, 0xD9, 0xC0, 0x0C, 0x92, 0x73, 0xD4, 0x1A, 0xF2 };

    readonly RijndaelManaged rj = new RijndaelManaged();

    public Crypto()
    {
    }

    // ************************ CRYPTER(Textebrut)*******************************
    /// <summary>
    /// Fonction de cryptage : elle necessite en argument une chaîne de caractères,
    /// et renvoie une chaîne de caractères cryptée (cipher-text).
    /// <param name=" TexteBrut"></param>
    /// <returns name="string CypherTexte"></returns>
    /// </summary>
    // ***************************************************************************

    public string Crypter(string TexteBrut)
    {
      if (TexteBrut.Length == 0) return string.Empty;
      MemoryStream cypherTexteMem = new MemoryStream();

      CryptoStream cStream = new CryptoStream(cypherTexteMem,
      rj.CreateEncryptor(clef, vect), CryptoStreamMode.Write);

      byte[] textebrutByte = new UnicodeEncoding().GetBytes(TexteBrut);

      cStream.Write(textebrutByte, 0, textebrutByte.Length);
      cStream.Close();

      byte[] cypherTexteByte = cypherTexteMem.ToArray();

      cypherTexteMem.Close();
      string cypherTexte = new UnicodeEncoding().GetString(cypherTexteByte);

      return cypherTexte;
    }


    // ************************ DECRYPTER(Textebrut)*****************************
    /// <summary>
    /// Fonction de décryptage : elle necessite en argument une chaîne de 
    /// caractères cryptés (cipher-text) et renvoie une chaîne de caractères.
    /// <param name="cypherTexte"></param>
    /// <returns name="Textebrut"></returns>
    /// </summary>
    // ***************************************************************************

    public string Decrypter(string cypherTexte)
    {
      if (cypherTexte.Length == 0)
        return string.Empty;
      MemoryStream cypherTexteMem = new MemoryStream(new UnicodeEncoding().GetBytes(cypherTexte));
      CryptoStream cStream = new CryptoStream(cypherTexteMem, rj.CreateDecryptor(clef, vect), CryptoStreamMode.Read);
      MemoryStream textebrutMem = new MemoryStream();

      do
      {
        var buf = new byte[100];

        Int32 bytesLus = cStream.Read(buf, 0, 100);

        if (0 == bytesLus)
          break;

        textebrutMem.Write(buf, 0, bytesLus);

      } while (true);

      cStream.Close();
      cypherTexteMem.Close();

      byte[] textebrutByte = textebrutMem.ToArray();

      textebrutMem.Close();

      string textebrut = new UnicodeEncoding().GetString(textebrutByte);
      return textebrut;
    }
  }
}
