/*
SELECT * FROM yw_VIEW_IDCenter_ORG_PERSONDEPT

SELECT * FROM _TCAUser
*/
SELECT 
A.EmpID --���
, A.EmpSeq --��������ڵ�
, A.CellPhone --�޴�����ȣ
, C.PwdMailAdder --�̸���
, D.EmpEngFirstName
, D.EmpEngLastName
, D.EmpName
, B.DeptSeq
, CASE WHEN CHARINDEX('@', C.PwdMailAdder) > 1 THEN substring(C.PwdMailAdder, 1, CHARINDEX('@', C.PwdMailAdder)-1) ELSE '' END PersonId -- ���̵� nvarchar(20)  
FROM _TDAEmpIn AS A WITH(NOLOCK)
INNER JOIN _THRAdmOrdEmp AS B WITH(NOLOCK) ON A.CompanySeq = B.CompanySeq AND B.IsOrdDateLast = '1' AND A.EmpSeq = B.EmpSeq
	AND B.OrdDate <= CASE WHEN ISNULL(A.RetireDate,'') > CONVERT(VARCHAR(8), GETDATE(), 112) THEN CONVERT(VARCHAR(8), GETDATE(), 112)  ELSE A.RetireDate  END  
	AND B.OrdEndDate >= CASE WHEN ISNULL(A.RetireDate,'') > CONVERT(VARCHAR(8), GETDATE(), 112) THEN CONVERT(VARCHAR(8), GETDATE(), 112)  ELSE A.RetireDate  END  
INNER JOIN _TCAUser AS C WITH(NOLOCK) ON A.CompanySeq = C.CompanySeq AND A.EmpSeq = C.EmpSeq AND C.PwdMailAdder <> ''  --PwdMailAdder(�̸���)�� NULL�� �ƴϾ�� HR ������ �ִ� ����.
INNER JOIN _TDAEmp AS D WITH(NOLOCK) ON A.CompanySeq = D.CompanySeq AND A.EmpSeq = D.EmpSeq
WHERE 1=1 
	AND C.UserSeq NOT IN (1)  --������ ����
	AND A.RetireDate >= CONVERT(VARCHAR(8), GETDATE(), 112) --������� RetireDate�� ���� ����� ���
ORDER BY A.EmpID ASC

