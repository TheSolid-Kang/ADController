/*
SELECT * FROM yw_VIEW_IDCenter_ORG_PERSONDEPT

SELECT * FROM _TCAUser
*/
SELECT 
A.EmpID --사번
, A.EmpSeq --사원내부코드
, A.CellPhone --휴대폰번호
, C.PwdMailAdder --이메일
, D.EmpEngFirstName
, D.EmpEngLastName
, D.EmpName
, B.DeptSeq
, CASE WHEN CHARINDEX('@', C.PwdMailAdder) > 1 THEN substring(C.PwdMailAdder, 1, CHARINDEX('@', C.PwdMailAdder)-1) ELSE '' END PersonId -- 아이디 nvarchar(20)  
FROM _TDAEmpIn AS A WITH(NOLOCK)
INNER JOIN _THRAdmOrdEmp AS B WITH(NOLOCK) ON A.CompanySeq = B.CompanySeq AND B.IsOrdDateLast = '1' AND A.EmpSeq = B.EmpSeq
	AND B.OrdDate <= CASE WHEN ISNULL(A.RetireDate,'') > CONVERT(VARCHAR(8), GETDATE(), 112) THEN CONVERT(VARCHAR(8), GETDATE(), 112)  ELSE A.RetireDate  END  
	AND B.OrdEndDate >= CASE WHEN ISNULL(A.RetireDate,'') > CONVERT(VARCHAR(8), GETDATE(), 112) THEN CONVERT(VARCHAR(8), GETDATE(), 112)  ELSE A.RetireDate  END  
INNER JOIN _TCAUser AS C WITH(NOLOCK) ON A.CompanySeq = C.CompanySeq AND A.EmpSeq = C.EmpSeq AND C.PwdMailAdder <> ''  --PwdMailAdder(이메일)이 NULL이 아니어야 HR 계정이 있는 것임.
INNER JOIN _TDAEmp AS D WITH(NOLOCK) ON A.CompanySeq = D.CompanySeq AND A.EmpSeq = D.EmpSeq
WHERE 1=1 
	AND C.UserSeq NOT IN (1)  --마스터 제외
	AND A.RetireDate >= CONVERT(VARCHAR(8), GETDATE(), 112) --현재기준 RetireDate가 없는 사원만 출력
ORDER BY A.EmpID ASC

