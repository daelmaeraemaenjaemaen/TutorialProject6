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
    int[] longPartNm = new int[6];
    int[] longPartbt = new int[6];

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
                        switch (words[1])
                        {
                            case "s":
                                nb1.is1 = true;
                                nb1.tick1 = 0;
                                nb1.isVisible1 = true;
                                totalNote++;
                                break;
                            case "l":
                                nb1.is1 = true;
                                nb1.tick1 = 1;
                                longPartNm[0] = partCount;
                                longPartbt[0] = count;
                                nb1.isVisible1 = true;
                                totalNote++;
                                break;
                            case "le":
                                nb1.is1 = false;
                                nb1.tick1 = 0;
                                countTick(0, count);
                                nb1.isVisible1 = true;
                                totalNote++; // 롱노트 1개분 추가
                                break;
                            case "!s":
                                nb1.is1 = true;
                                nb1.tick1 = 0;
                                totalNote++;
                                nb1.isVisible1 = false;
                                break;
                            case "!l":
                                nb1.is1 = true;
                                nb1.tick1 = 1;
                                longPartNm[0] = partCount;
                                longPartbt[0] = count;
                                nb1.isVisible1 = false;
                                totalNote++;
                                break;
                            case "!le":
                                nb1.is1 = false;
                                nb1.tick1 = 0;
                                countTick(0, count);
                                nb1.isVisible1 = false;
                                totalNote++; // 롱노트 1개분 추가
                                break;
                            default:
                                nb1.is1 = false;
                                nb1.tick1 = 0;
                                break;
                        }

                        switch (words[2])
                        {
                            case "s":
                                nb1.is2 = true;
                                nb1.tick2 = 0;
                                nb1.isVisible2 = true;
                                totalNote++;
                                break;
                            case "l":
                                nb1.is2 = true;
                                nb1.tick2 = 1;
                                longPartNm[1] = partCount;
                                longPartbt[1] = count;
                                nb1.isVisible2 = true;
                                totalNote++;
                                break;
                            case "le":
                                nb1.is2 = false;
                                nb1.tick2 = 0;
                                countTick(1, count);
                                nb1.isVisible2 = true;
                                totalNote++;
                                break;
                            case "!s":
                                nb1.is2 = true;
                                nb1.tick2 = 0;
                                totalNote++;
                                nb1.isVisible2 = false;
                                break;
                            case "!l":
                                nb1.is2 = true;
                                nb1.tick2 = 1;
                                longPartNm[1] = partCount;
                                longPartbt[1] = count;
                                nb1.isVisible2 = false;
                                totalNote++;
                                break;
                            case "!le":
                                nb1.is2 = false;
                                nb1.tick2 = 0;
                                countTick(1, count);
                                nb1.isVisible2 = false;
                                totalNote++; // 롱노트 1개분 추가
                                break;
                            default:
                                nb1.is2 = false;
                                nb1.tick2 = 0;
                                break;
                        }

                        switch (words[3])
                        {
                            case "s":
                                nb1.is3 = true;
                                nb1.tick3 = 0;
                                nb1.isVisible3 = true;
                                totalNote++;
                                break;
                            case "l":
                                nb1.is3 = true;
                                nb1.tick3 = 1;
                                longPartNm[2] = partCount;
                                longPartbt[2] = count;
                                nb1.isVisible3 = true;
                                totalNote++;
                                break;
                            case "le":
                                nb1.is3 = false;
                                nb1.tick3 = 0;
                                countTick(2, count);
                                nb1.isVisible3 = true;
                                totalNote++;
                                break;
                            case "!s":
                                nb1.is3 = true;
                                nb1.tick3 = 0;
                                totalNote++;
                                nb1.isVisible3 = false;
                                break;
                            case "!l":
                                nb1.is3 = true;
                                nb1.tick3 = 1;
                                longPartNm[2] = partCount;
                                longPartbt[2] = count;
                                nb1.isVisible3 = false;
                                totalNote++;
                                break;
                            case "!le":
                                nb1.is3 = false;
                                nb1.tick3 = 0;
                                countTick(2, count);
                                nb1.isVisible3 = false;
                                totalNote++; // 롱노트 1개분 추가
                                break;
                            default:
                                nb1.is3 = false;
                                nb1.tick3 = 0;
                                break;
                        }
                    }

                    if (BeatConvert(beatR) > count)
                    {
                        switch (words[4])
                        {
                            case "s":
                                nb2.is1 = true;
                                nb2.tick1 = 0;
                                nb2.isVisible1 = true;
                                totalNote++;
                                break;
                            case "l":
                                nb2.is1 = true;
                                nb2.tick1 = 1;
                                longPartNm[3] = partCount;
                                longPartbt[3] = count;
                                nb2.isVisible1 = true;
                                totalNote++;
                                break;
                            case "le":
                                nb2.is1 = false;
                                nb2.tick1 = 0;
                                countTick(3, count);
                                nb2.isVisible1 = true;
                                totalNote++;
                                break;
                            case "!s":
                                nb2.is1 = true;
                                nb2.tick1 = 0;
                                nb2.isVisible1 = false;
                                totalNote++;
                                break;
                            case "!l":
                                nb2.is1 = true;
                                nb2.tick1 = 1;
                                longPartNm[3] = partCount;
                                longPartbt[3] = count;
                                nb2.isVisible1 = false;
                                totalNote++;
                                break;
                            case "!le":
                                nb2.is1 = false;
                                nb2.tick1 = 0;
                                countTick(3, count);
                                nb2.isVisible1 = false;
                                totalNote++;
                                break;
                            default:
                                nb2.is1 = false;
                                nb2.tick1 = 0;
                                break;
                        }

                        switch (words[5])
                        {
                            case "s":
                                nb2.is2 = true;
                                nb2.tick2 = 0;
                                nb2.isVisible2 = true;
                                totalNote++;
                                break;
                            case "l":
                                nb2.is2 = true;
                                nb2.tick2 = 1;
                                longPartNm[4] = partCount;
                                longPartbt[4] = count;
                                nb2.isVisible2 = true;
                                totalNote++;
                                break;
                            case "le":
                                nb2.is2 = false;
                                nb2.tick2 = 0;
                                countTick(4, count);
                                nb2.isVisible2 = true;
                                totalNote++;
                                break;
                            case "!s":
                                nb2.is2 = true;
                                nb2.tick2 = 0;
                                nb2.isVisible2 = false;
                                totalNote++;
                                break;
                            case "!l":
                                nb2.is2 = true;
                                nb2.tick2 = 1;
                                longPartNm[4] = partCount;
                                longPartbt[4] = count;
                                nb2.isVisible2 = false;
                                totalNote++;
                                break;
                            case "!le":
                                nb2.is2 = false;
                                nb2.tick2 = 0;
                                countTick(4, count);
                                nb2.isVisible2 = false;
                                totalNote++;
                                break;
                            default:
                                nb2.is2 = false;
                                nb2.tick2 = 0;
                                break;
                        }

                        switch (words[6])
                        {
                            case "s":
                                nb2.is3 = true;
                                nb2.tick3 = 0;
                                nb2.isVisible3 = true;
                                totalNote++;
                                break;
                            case "l":
                                nb2.is3 = true;
                                nb2.tick3 = 1;
                                longPartNm[5] = partCount;
                                longPartbt[5] = count;
                                nb2.isVisible3 = true;
                                totalNote++;
                                break;
                            case "le":
                                nb2.is3 = false;
                                nb2.tick3 = 0;
                                countTick(5, count);
                                nb2.isVisible3 = true;
                                totalNote++;
                                break;
                            case "!s":
                                nb2.is3 = true;
                                nb2.tick3 = 0;
                                nb2.isVisible3 = false;
                                totalNote++;
                                break;
                            case "!l":
                                nb2.is3 = true;
                                nb2.tick3 = 1;
                                longPartNm[5] = partCount;
                                longPartbt[5] = count;
                                nb2.isVisible3 = false;
                                totalNote++;
                                break;
                            case "!le":
                                nb2.is3 = false;
                                nb2.tick3 = 0;
                                countTick(5, count);
                                nb2.isVisible3 = false;
                                totalNote++;
                                break;
                            default:
                                nb2.is3 = false;
                                nb2.tick3 = 0;
                                break;
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

    private void countTick(int line, int lastCount)
    {
        //틱 수 = 중간에 낀 파트 수 * 2 + 첫 파트 진행 틱 + 막파트 진행 틱
        //첫 파트 진행 틱 = (총 인덱스 - nb 인덱스) / 해당 파트 비트 * 8
        //막파트 진행 틱 = nb 인덱스 / 해당 파트 비트 * 8
        int firstBeat = line > 3 ? noteParts[longPartNm[line]].beatL : noteParts[longPartNm[line]].beatR;
        int lastBeat = line > 3 ? noteParts[partCount].beatL : noteParts[partCount].beatR;
        float firstTick = BeatConvert(firstBeat) - longPartbt[line] / firstBeat * 8;
        float lastTick = lastCount / lastBeat * 8;
        float tick = (partCount - longPartNm[line] - 1) * 2 + firstTick + lastTick - 1;
        switch (line)
        {
            case 0:
                noteParts[longPartNm[line]].noteL[longPartbt[line]].tick1 = tick;
                break;
            case 1:
                noteParts[longPartNm[line]].noteL[longPartbt[line]].tick2 = tick;
                break;
            case 2:
                noteParts[longPartNm[line]].noteL[longPartbt[line]].tick3 = tick;
                break;
            case 3:
                noteParts[longPartNm[line]].noteR[longPartbt[line]].tick1 = tick;
                break;
            case 4:
                noteParts[longPartNm[line]].noteR[longPartbt[line]].tick2 = tick;
                break;
            case 5:
                noteParts[longPartNm[line]].noteR[longPartbt[line]].tick3 = tick;
                break;
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
    public bool is1;
    public bool is2;
    public bool is3;

    // 노트가 보이는가?
    public bool isVisible1;
    public bool isVisible2;
    public bool isVisible3;

    // 단노트: 0, 롱노트: 1 이상
    public float tick1;
    public float tick2;
    public float tick3;
}