// See https://aka.ms/new-console-template for more information
using ADController._01.CSyncModule;
using ADController._97.DesignPattern;
using Engine;
using Engine._10.CActiveDirectoryMgr;

var mgrInstance = CSyncModuleMgr.GetInstance();

#if DEBUG
do
{
    PrintTitle();
    int num = 0;
    num = CIO.AskAndReturnInteger();
    if (num == 99)
        break;
} while (99 != mgrInstance.Excute((int)CSyncModuleMgr.SYNC_MODEL.TEST));

void PrintTitle()
{
    var titles = new List<string>();
    titles.Add("화면 선택");
    titles.Add("1. 테스트페이지");
    titles.Add("99.EXIT");
    titles.ForEach(x => { Console.WriteLine(x); });
}
#else
int i = (int)CSyncModuleMgr.SYNC_MODEL.AD_USER;
while ((int)CSyncModuleMgr.SYNC_MODEL.TEST < i && (int)CSyncModuleMgr.SYNC_MODEL.END > i)
    mgrInstance.Excute(i++);
#endif

/*
 * 두환 차장님 조건
1. 용어 변경
    가. 동기화 객체명 변경: SyncModule -> SyncModule
2. 사용하지 않아도 되는 함수 삭제
    가. CSyncModule 클래스 내의 옵저버 패턴 관련 default함수 삭제
        1) protected void InitFunc();
        2) protected void InitPracticeFunc();
    나. CObj 클래스 사용하지 않으므로 삭제
 */