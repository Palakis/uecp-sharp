using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UECP
{
	public class RdsE1Encoding : CharMapEncoding
	{
		public override string WebName => "RDS E.1";

		public RdsE1Encoding() : base(99123)
		{
			var e1 = new Dictionary<char, byte> {
				{' ', 0x20 }, // espace
				{'!', 0x21 }, // point d'exclamation
				{'"', 0x22 }, // guillemets anglais
				{'#', 0x23 }, // dièse
				{'¤', 0x24 }, // "currency sign"
				{'%', 0x25 }, // pour cent
				{'&', 0x26 }, // esperluette
				{'\'', 0x27 }, // apostrophe
				{'(', 0x28 }, // parenthèse ouvrante
				{')', 0x29 }, // parenthèse fermante
				{'*', 0x2A }, // astérisque
				{'+', 0x2B }, // plus
				{',', 0x2C }, // virgule
				{'-', 0x2D }, // tiret
				{'.', 0x2E }, // point
				{'/', 0x2F }, // slash

				{'0', 0x30 }, // zéro
				{'1', 0x31 }, // un
				{'2', 0x32 }, // deux
				{'3', 0x33 }, // trois
				{'4', 0x34 }, // quatre
				{'5', 0x35 }, // cinq
				{'6', 0x36 }, // six
				{'7', 0x37 }, // sept
				{'8', 0x38 }, // huit
				{'9', 0x39 }, // neuf
				{':', 0x3A }, // deux-points
				{';', 0x3B }, // point-virgule
				{'<', 0x3C }, // chevron ouvrant
				{'=', 0x3D }, // signe égal
				{'>', 0x3E }, // chevron fermant
				{'?', 0x3F }, // point d'interrogation

				{'@', 0x40 }, // arobase
				{'A', 0x41 }, // A majuscule
				{'B', 0x42 }, // B majuscule
				{'C', 0x43 }, // C majuscule
				{'D', 0x44 }, // D majuscule
				{'E', 0x45 }, // E majuscule
				{'F', 0x46 }, // F majuscule
				{'G', 0x47 }, // G majuscule
				{'H', 0x48 }, // H majuscule
				{'I', 0x49 }, // I majuscule
				{'J', 0x4A }, // J majuscule
				{'K', 0x4B }, // K majuscule
				{'L', 0x4C }, // L majuscule
				{'M', 0x4D }, // M majuscule
				{'N', 0x4E }, // N majuscule
				{'O', 0x4F }, // O majuscule

				{'P', 0x50 }, // P majuscule
				{'Q', 0x51 }, // Q majuscule
				{'R', 0x52 }, // R majuscule
				{'S', 0x53 }, // S majuscule
				{'T', 0x54 }, // T majuscule
				{'U', 0x55 }, // U majuscule
				{'V', 0x56 }, // V majuscule
				{'W', 0x57 }, // W majuscule
				{'X', 0x58 }, // X majuscule
				{'Y', 0x59 }, // Y majuscule
				{'Z', 0x5A }, // Z majuscule
				{'[', 0x5B }, // braquet ouvrant
				{'\\', 0x5C }, // anti slash
				{']', 0x5D }, // braquet fermant
				{'─', 0x5E },
				// {'', 0x5F }, // ??

				{'║', 0x60 },
				{'a', 0x61 }, // a minuscule
				{'b', 0x62 }, // b minuscule
				{'c', 0x63 }, // c minuscule
				{'d', 0x64 }, // d minuscule
				{'e', 0x65 }, // e minuscule
				{'f', 0x66 }, // f minuscule
				{'g', 0x67 }, // g minuscule
				{'h', 0x68 }, // h minuscule
				{'i', 0x69 }, // i minuscule
				{'j', 0x6A }, // j minuscule
				{'k', 0x6B }, // k minuscule
				{'l', 0x6C }, // l minuscule
				{'m', 0x6D }, // m minuscule
				{'n', 0x6E }, // n minuscule
				{'o', 0x6F }, // o minuscule

				{'p', 0x70 }, // p minuscule
				{'q', 0x71 }, // q minuscule
				{'r', 0x72 }, // r minuscule
				{'s', 0x73 }, // s minuscule
				{'t', 0x74 }, // t minuscule
				{'u', 0x75 }, // u minuscule
				{'v', 0x76 }, // v minuscule
				{'w', 0x77 }, // w minuscule
				{'x', 0x78 }, // x minuscule
				{'y', 0x79 }, // y minuscule
				{'z', 0x7A }, // z minuscule
				// {'', 0x7B }, // ??
				{'│', 0x7C },
				// {'', 0x7D }, // ??
				// {'', 0x7E }, // ??
				// {' ', 0x7F }, // vide

				{'á', 0x80}, // a accent aigu minuscule
				{'à', 0x81}, // a accent grave minuscule
				{'é', 0x82}, // e accent aigu minuscule
				{'è', 0x83}, // e accent grave minuscule
				{'í', 0x84}, // i accent aigu minuscule
				{'ì', 0x85}, // i accent grave minuscule
				{'ó', 0x86}, // o accent aigu minuscule
				{'ò', 0x87}, // o accent grave minuscule
				{'ú', 0x88}, // u accent aigu minuscule
				{'ù', 0x89}, // u accent grave minuscule
				{'Ñ', 0x8A}, // n tilde majuscule
				{'Ç', 0x8B}, // c cédille majuscule
				{'Ş', 0x8C}, // s cédille majuscule
				{'ß', 0x8D}, // eszet
				{'İ', 0x8E}, // i majuscule point en chef
				{'Ĳ', 0x8F}, // digramme soudé majuscule de IJ

				{'â', 0x90}, // a circonflexe minuscule
				{'ä', 0x91}, // a trëma miniscule
				{'ê', 0x92}, // e circonflexe minuscule
				{'ë', 0x93}, // e trëma minuscule
				{'î', 0x94}, // i circonflexe minuscule
				{'ï', 0x95}, // i trëma minuscule
				{'ô', 0x96}, // o circonflexe minuscule
				{'ö', 0x97}, // o trëma minuscule
				{'û', 0x98}, // u circonflexe minuscule
				{'ü', 0x99}, // u trëma minuscule
				{'ñ', 0x9A}, // n tilde minuscule
				{'ç', 0x9B}, // c cédille minuscule
				{'ş', 0x9C}, // s cédille minuscule
				// {' ', 0x9D}, // ??
				// {' ', 0x9E}, // ??
				{'ĳ', 0x9F}, // digramme soudé minuscule de ij

				// {'', 0xA0}, // ??
				// {'', 0xA1}, // ??
				{'©', 0xA2}, // symbole copyright
				{'‰', 0xA3}, // pour mille
				{'Ğ', 0xA4}, // g breve majuscule
				{'ě', 0xA5}, // e caron minuscule
				{'ň', 0xA6}, // n caron minuscule
				{'ő', 0xA7}, // o double accent aigu minuscule
				{'π', 0xA8}, // pi
				// {'', 0xA9}, // ??
				{'£', 0xAA}, // livre sterling
				{'$', 0xAB}, // dollar
				{'←', 0xAC}, // fleche gauche
				{'↑', 0xAD}, // fleche haut
				{'→', 0xAE}, // fleche droite
				{'↓', 0xAF}, // fleche bas

				// {'', 0xB0}, // ??
				{'¹', 0xB1}, // exposant 1
				{'²', 0xB2}, // exposant 2
				{'³', 0xB3}, // exposant 3
				{'±', 0xB4}, // symbole plus moins (tolérance)
				// {'', 0xB5}, ??
				{'ń', 0xB6}, // n accent aigu
				{'ű', 0xB7}, // u double accent aigu minuscule
				// {'', 0xB8}, // ??
				{'¿', 0xB9}, // point d'interrogation à l'envers
				{'÷', 0xBA}, // division
				{'°', 0xBB}, // degré
				{'¼', 0xBC}, // un quart
				{'½', 0xBD}, // un demi
				{'¾', 0xBE}, // trois quarts
				{'§', 0xBF}, // paragraphe

				{'Á', 0xC0}, // a accent aigu majuscule
				{'À', 0xC1}, // a accent grave majuscule
				{'É', 0xC2}, // e accent aigu majuscule
				{'È', 0xC3}, // e accent grave majuscule
				{'Í', 0xC4}, // i accent aigu majuscule
				{'Ì', 0xC5}, // i accent grave majuscule
				{'Ó', 0xC6}, // o accent aigu majuscule
				{'Ò', 0xC7}, // o accent grave majuscule
				{'Ú', 0xC8}, // u accent aigu majuscule
				{'Ù', 0xC9}, // u accent grave majuscule
				{'Ř', 0xCA}, // r caron majuscule
				{'Č', 0xCB}, // c caron majuscule
				{'Š', 0xCC}, // s caron majuscule
				{'Ž', 0xCD}, // z caron majuscule
				{'Ð', 0xCE}, // ed islandais majuscule
				// {'', 0xCF}, // ??

				// {'', 0xD0}, // ??
				{'Ä', 0xD1}, // a trema majuscule
				{'Ê', 0xD2}, // e circonflexe majuscule
				{'Ë', 0xD3}, // e trema majuscule
				{'Î', 0xD4}, // i circonflexe majuscule
				{'Ï', 0xD5}, // i trema majuscule
				{'Ô', 0xD6}, // o circonflexe majuscule
				{'Ö', 0xD7}, // o trema majuscule
				{'Û', 0xD8}, // u circonflexe majuscule
				{'Ü', 0xD9}, // u trema majuscule
				{'ř', 0xDA}, // r caron minuscule
				{'č', 0xDB}, // c caron minuscule
				{'š', 0xDC}, // s caron minuscule
				{'ž', 0xDD}, // z caron minuscule
				{'đ', 0xDE}, // d barré
				// {'', 0xDF}, // ??

				{'Ã', 0xE0}, // a tilde majuscule
				{'Å', 0xE1}, // a degré majuscule
				{'Æ', 0xE2}, // e dans l'a majuscule
				{'Œ', 0xE3}, // e dans l'o majuscule
				{'ŷ', 0xE4}, // y circonflexe minuscule
				{'Ý', 0xE5}, // y accent aigu majuscule
				{'Õ', 0xE6}, // o tilde majuscule
				{'Ø', 0xE7}, // o barré majuscule
				{'Ꝥ', 0xE8}, // thorn barré majuscule
				// {'', 0xE9}, // ??
				{'Ŕ', 0xEA}, // r accent aigu majuscule
				{'Ć', 0xEB}, // c accent aigu majuscule
				{'Ś', 0xEC}, // s accent aigu majuscule
				{'Ź', 0xED}, // z accent aigu majuscule
				// {'', 0xEE}, // ??
				// {'', 0xEF}, // ??

				{'ã', 0xF0}, // a tilde minuscule
				{'å', 0xF1}, // a degré minuscule
				{'æ', 0xF2}, // e dans l'a minuscule
				{'œ', 0xF3}, // e dans l'o minuscule
				{'ŵ', 0xF4}, // w circonflexe minuscule
				{'ý', 0xF5}, // y accent aigu minuscule
				{'õ', 0xF6}, // o tilde minuscule
				{'ø', 0xF7}, // o barré minuscule
				{'ꝧ', 0xF8}, // thorn barré minuscule avec jambage
				// {'', 0xF9}, // ??
				{'ŕ', 0xFA}, // r accent aigu minuscule
				{'ć', 0xFB}, // c accent aigu minuscule
				{'ś', 0xFC}, // s accent aigu minuscule
				{'ź', 0xFD}, // z accent aigu minuscule
				// {'', 0xFE} // ??
			};

			SetCharMap(e1);
		}
	}
}
