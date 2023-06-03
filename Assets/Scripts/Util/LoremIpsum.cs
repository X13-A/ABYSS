using UnityEngine;
using System.Collections;
using System;
using System.Linq;

public class LoremIpsum
{
    public static char GeneratePseudoRandomChar(System.Random rand)
    {
        string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return chars[rand.Next(chars.Length)];
    }

    public static string Generate(int size)
    {
        System.Random rand = new System.Random();
        return new string(Enumerable.Range(0, 30).Select(_ => GeneratePseudoRandomChar(rand)).ToArray());
    }
}
