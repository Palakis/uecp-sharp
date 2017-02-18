/*
MIT License

Copyright (c) 2017 Stéphane Lepin <stephane.lepin@gmail.com>

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

namespace UECP
{
    public enum MEC
    {
        RDS_PI = 0x01,
        RDS_PS = 0x02,
        RDS_PIN = 0x06,
        RDS_DI_PTY = 0x04,
        RDS_TA_TP = 0x03,
        RDS_MS = 0x05,
        RDS_PTY = 0x07,
        RDS_PTYN = 0x3E,
        RDS_RT = 0x0A,
        RDS_AF = 0x13,
        RDS_EON_AF = 0x14,
        RDS_SLOW_LABELING = 0x1A,
        RDS_LINKAGE_INFO = 0x2E
    }

    public enum PTY
    {
        None = 0,
        News = 1,
        CurrentAffairs = 2,
        Information = 3,
        Sport = 4,
        Education = 5,
        Drama = 6,
        Culture = 7,
        Science = 8,
        Varied = 9,
        PopMusic = 10,
        RockMusic = 11,
        EasyListening = 12,
        LightClassical = 13,
        SeriousClassical = 14,
        OtherMusic = 15,
        Weather = 16,
        Finance = 17,
        Children = 18,
        SocialAffairs = 19,
        Religion = 20,
        PhoneIn = 21,
        Travel = 22,
        Leisure = 23,
        JazzMusic = 24,
        CountryMusic = 25,
        NationalMusic = 26,
        OldiesMusic = 27,
        FolkMusic = 28,
        Documentary = 29,
        AlarmTest = 30,
        Alarm = 31
    }
}
