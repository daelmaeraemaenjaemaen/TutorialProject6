using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;

public class PatternReader
{
    int beatL = 0;
    int beatR = 0;
    List<NotePart> noteParts = new();
    int partCount = -1;
    int[] longPartNm = { -1, -1, -1, -1, -1, -1 };
    int[] longPartbt = { -1, -1, -1, -1, -1, -1 };
    float[] longtick = { 0, 0, 0, 0, 0, 0 };

    public List<NotePart> ReadPattern(string fileName)
    {
        string path = Application.dataPath + "/Resources/Pattern/" + fileName;
        StreamReader sr = new(path);
        int totalNote = 0; // 롱노트는 1개로 간주

        try
        {
            while (!sr.EndOfStream)
            {
                string line = sr.ReadLine();
                string[] words = line.Split(' ');
                if (words.Length == 5) // Part 작성
                {
                    NotePart np = new();
                    np.partNum = Convert.ToInt32(words[0]);
                    np.beatNum = Convert.ToInt32(words[1]);
                    np.beatL = Convert.ToInt32(words[2]);
                    np.beatR = Convert.ToInt32(words[3]);
                    np.gim = words[4];
                    np.noteL = new NoteBeat[BeatConvert(np.beatL)];
                    np.noteR = new NoteBeat[BeatConvert(np.beatR)];
                    noteParts.Add(np);

                    beatL = np.beatL;
                    beatR = np.beatR;
                    partCount++;
                }
                else // Beat 작성
                {
                    int count = Convert.ToInt32(words[0]) - 1;
                    NoteBeat nb1 = new();
                    NoteBeat nb2 = new();
                    if (BeatConvert(beatL) > count)
                    {
                        for (int i = 0; i <= 2; i++)
                        {
                            switch (words[i + 1])
                            {
                                case "s":
                                    nb1.isexist[i] = true;
                                    nb1.tick[i] = 0;
                                    nb1.isVisible[i] = true;
                                    totalNote++;
                                    break;
                                case "l":
                                    nb1.isexist[i] = true;
                                    nb1.tick[i] = 1;
                                    longPartNm[i] = partCount;
                                    longPartbt[i] = count;
                                    nb1.isVisible[i] = true;
                                    totalNote++;
                                    break;
                                case "le":
                                    nb1.isexist[i] = true;
                                    nb1.tick[i] = 0;
                                    CountTick(i);
                                    nb1.isVisible[i] = true;
                                    totalNote++; // 롱노트 1개분 추가
                                    break;
                                case "!s":
                                    nb1.isexist[i] = true;
                                    nb1.tick[i] = 0;
                                    totalNote++;
                                    nb1.isVisible[i] = false;
                                    break;
                                case "!l":
                                    nb1.isexist[i] = true;
                                    nb1.tick[i] = 1;
                                    longPartNm[i] = partCount;
                                    longPartbt[i] = count;
                                    nb1.isVisible[i] = false;
                                    totalNote++;
                                    break;
                                case "!le":
                                    nb1.isexist[i] = false;
                                    nb1.tick[i] = 0;
                                    CountTick(i);
                                    nb1.isVisible[i] = false;
                                    totalNote++; // 롱노트 1개분 추가
                                    break;
                                default:
                                    nb1.isexist[i] = false;
                                    nb1.tick[i] = 0;
                                    break;
                            }
                            if (longPartNm[i] != -1 && longPartbt[i] != -1)
                            {
                                longtick[i] += 8f / beatL; // 틱 간격 비트/현재 비트 수
                            }
                        }
                    }

                    if (BeatConvert(beatR) > count)
                    {
                        for (int i = 0; i <= 2; i++)
                        {
                            switch (words[i + 4])
                            {
                                case "s":
                                    nb2.isexist[i] = true;
                                    nb2.tick[i] = 0;
                                    nb2.isVisible[i] = true;
                                    totalNote++;
                                    break;
                                case "l":
                                    nb2.isexist[i] = true;
                                    nb2.tick[i] = 1;
                                    longPartNm[i + 3] = partCount;
                                    longPartbt[i + 3] = count;
                                    nb2.isVisible[i] = true;
                                    totalNote++;
                                    break;
                                case "le":
                                    nb2.isexist[i] = true;
                                    nb2.tick[i] = 0;
                                    CountTick(i + 3);
                                    nb2.isVisible[i] = true;
                                    totalNote++; // 롱노트 1개분 추가
                                    break;
                                case "!s":
                                    nb2.isexist[i] = true;
                                    nb2.tick[i] = 0;
                                    totalNote++;
                                    nb2.isVisible[i] = false;
                                    break;
                                case "!l":
                                    nb2.isexist[i] = true;
                                    nb2.tick[i] = 1;
                                    longPartNm[i + 3] = partCount;
                                    longPartbt[i + 3] = count;
                                    nb2.isVisible[i] = false;
                                    totalNote++;
                                    break;
                                case "!le":
                                    nb2.isexist[i] = false;
                                    nb2.tick[i] = 0;
                                    CountTick(i + 3);
                                    nb2.isVisible[i] = false;
                                    totalNote++; // 롱노트 1개분 추가
                                    break;
                                default:
                                    nb2.isexist[i] = false;
                                    nb2.tick[i] = 0;
                                    break;
                            }
                            if (longPartNm[i + 3] != -1 && longPartbt[i + 3] != -1)
                            {
                                longtick[i + 3] += 8f / beatL; // 틱 간격 비트/현재 비트 수
                            }
                        }
                    }

                    noteParts[partCount].noteL[count] = nb1;
                    noteParts[partCount].noteR[count] = nb2;
                }
            }
        }
        finally
        {
            sr.Close();
        }

        Score.setBaseScore(totalNote);
        return noteParts;
    }

    private int BeatConvert(int i)
    {
        return i switch
        {
            4 => 1,
            8 => 2,
            12 => 3,
            16 => 4,
            24 => 6,
            32 => 8,
            _ => -1,// error
        };
    }

    private void CountTick(int line)
    {
        if (line <= 2) // noteL
        {
            noteParts[longPartNm[line]].noteL[longPartbt[line]].tick[line] = longtick[line];
            longPartNm[line] = -1;
            longPartbt[line] = -1;
            longtick[line] = 0;
        }
        else // noteR
        {
            noteParts[longPartNm[line]].noteR[longPartbt[line]].tick[line - 3] = longtick[line];
            longPartNm[line] = -1;
            longPartbt[line] = -1;
            longtick[line] = 0;
        }
    }
}

public class NotePart
{
    public int partNum;
    public int beatNum;
    public int beatL;
    public int beatR;
    public string gim;

    public NoteBeat[] noteL;
    public NoteBeat[] noteR;
}

public class NoteBeat
{
    // 노트가 존재하는가?
    public bool[] isexist = new bool[3];

    // 노트가 보이는가?
    public bool[] isVisible = new bool[3];

    // 단노트: 0, 롱노트: 1 이상
    public float[] tick = new float[3];
}