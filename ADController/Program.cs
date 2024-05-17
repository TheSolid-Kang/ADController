// See https://aka.ms/new-console-template for more information
using ADController._97.DesignPattern;
using Engine;
using Engine._10.CActiveDirectoryMgr;

Console.WriteLine("Hello, World!");

var mgrInstance = CSceneMgr.GetInstance();
int num = 0;
do
{
    PrintTitle();
    num = CIO.AskAndReturnInteger();
    if (num == 99)
        break;
    mgrInstance.SetScene(num);
} while (99 != mgrInstance.Excute(num));

void PrintTitle()
{
    var titles = new List<string>();
    titles.Add("화면 선택");
    titles.Add("1. 테스트페이지");
    titles.Add("2. ");
    titles.Add("99.EXIT");
    titles.ForEach(x => { Console.WriteLine(x); });
}

