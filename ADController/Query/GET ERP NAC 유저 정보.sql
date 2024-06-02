SELECT
--A.EmpID--사번																																													
--, A.EmpSeq--사원내부코드																																											
--, Z.CellPhone--휴대폰번호																																										
--, B.DeptSeq																																														
--, N'' AS PwdMailAdder																																											
--, N'' AS PersonId																																												
--, D.EmpEngFirstName																																												
--, D.EmpEngLastName																																												
--, D.EmpName																																														
--, D.EmpFamilyName --성																																											
--, D.EmpFirstName --이름																																											
--, G.MinorName AS 직급																																											
--, H.MinorName AS 직책

0 AS AdUserSeq
, N'' AS uSNCreated
, N'' AS description
, N'' AS distinguishedName
, N'' AS homePhone
, cast(N'1900-01-01 00:00:00.000'AS DateTime) AS lastLogon
, N'' AS otherHomePhone
, N'' AS otherMobile
, cast(N'1900-01-01 00:00:00.000'AS DateTime) AS pwdLastSet
, N'' AS sn
, N'' AS manager
, N'' AS sAMAccountName
, 0 AS userPrincipalName
, 0 AS uSNChanged
, cast(N'1900-01-01 00:00:00.000'AS DateTime) AS whenChanged
, cast(N'1900-01-01 00:00:00.000'AS DateTime) AS whenCreated

, CONCAT(D.EmpName, N'(', D.EmpEngFirstName, D.EmpEngLastName, N')') AS cn																														
, E.DeptName AS department																																										
, CONCAT(D.EmpName, N'(', D.EmpEngFirstName, D.EmpEngLastName, N')') AS displayName																												
, CONCAT(D.EmpName, N'(', D.EmpEngFirstName, D.EmpEngLastName, N')') AS givenName																												
, F.MinorName AS title --직위																																									
, N'' AS mail																																													
, A.EmpID AS sAMAccountName																																										
FROM _fnAdmEmpOrdRetResidId(1, CONVERT(NCHAR(8), GETDATE(), 112)) AS A																															
INNER JOIN _THRAdmOrdEmp AS B WITH(NOLOCK) ON B.IsOrdDateLast = '1' AND A.EmpSeq = B.EmpSeq																										
    AND B.OrdDate <= CASE WHEN ISNULL(A.RetireDate,'') > CONVERT(VARCHAR(8), GETDATE(), 112) THEN CONVERT(VARCHAR(8), GETDATE(), 112)  ELSE A.RetireDate END								
    AND B.OrdEndDate >= CASE WHEN ISNULL(A.RetireDate,'') > CONVERT(VARCHAR(8), GETDATE(), 112) THEN CONVERT(VARCHAR(8), GETDATE(), 112)  ELSE A.RetireDate END							
INNER JOIN _TDAEmp AS D WITH(NOLOCK) ON B.CompanySeq = D.CompanySeq AND A.EmpSeq = D.EmpSeq																										
INNER JOIN _TDADept AS E WITH(NOLOCK) ON B.DeptSeq = E.DeptSeq																																	
-- 직위를 가져오기 위한 조인																																										
LEFT OUTER JOIN _TDAUMinor AS F WITH(NOLOCK) ON (B.CompanySeq = F.CompanySeq AND  B.UMJpSeq = F.MinorSeq) 																						
-- 직급을 가져오기 위한 조인																																										
LEFT OUTER JOIN _TDAUMinor AS G WITH(NOLOCK) ON (B.CompanySeq = G.CompanySeq AND B.UMPgSeq = G.MinorSeq) 																						
-- 직책을 가져오기 위한 조인  																																									
LEFT OUTER JOIN _TDAUMinor AS H WITH(NOLOCK) ON (B.CompanySeq = H.CompanySeq AND  B.UMJdSeq    = H.MinorSeq)																					
																																																
INNER JOIN _TDAEmpIn AS Z WITH(NOLOCK) ON B.CompanySeq = Z.CompanySeq AND A.EmpId = Z.EmpId																										
WHERE 1=1																																														
	AND OrdName NOT IN(N'')																																										
	AND RetDate = N''																																											
	AND A.EmpId NOT IN																																											
	(																																															
		SELECT																																													
		A.EmpID--사번																																											
		FROM _TDAEmpIn AS A WITH(NOLOCK)																																						
		INNER JOIN _THRAdmOrdEmp AS B WITH(NOLOCK) ON A.CompanySeq = B.CompanySeq AND B.IsOrdDateLast = '1' AND A.EmpSeq = B.EmpSeq																
		    AND B.OrdDate <= CASE WHEN ISNULL(A.RetireDate,'') > CONVERT(VARCHAR(8), GETDATE(), 112) THEN CONVERT(VARCHAR(8), GETDATE(), 112)  ELSE A.RetireDate END						
		    AND B.OrdEndDate >= CASE WHEN ISNULL(A.RetireDate,'') > CONVERT(VARCHAR(8), GETDATE(), 112) THEN CONVERT(VARCHAR(8), GETDATE(), 112)  ELSE A.RetireDate END					
		INNER JOIN _TCAUser AS C WITH(NOLOCK) ON A.CompanySeq = C.CompanySeq AND A.EmpSeq = C.EmpSeq AND C.PwdMailAdder<> ''--PwdMailAdder(이메일)이 NULL이 아니어야 HR 계정이 있
		INNER JOIN _TDAEmp AS D WITH(NOLOCK) ON A.CompanySeq = D.CompanySeq AND A.EmpSeq = D.EmpSeq																								
		WHERE 1 = 1																																												
		    AND C.UserSeq NOT IN(1)--마스터 제외																																					
		    AND A.RetireDate >= CONVERT(VARCHAR(8), GETDATE(), 112)--현재기준 RetireDate가 없는 사원만 출력																						
	)																																															
ORDER BY A.EmpID ASC																																											