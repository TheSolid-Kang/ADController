/************************************************************  
설  명 - 사용자 동기화 조회     
작성일 - 2024년 05월 30일   
작성자 - 강태경  
************************************************************/  
CREATE   PROC [dbo].[yw_SSyncADUserListQuery]
@xmlDocument    NVARCHAR(MAX) = N'<ROOT> <> </ROOT>',  
@xmlFlags INT = 0,  
@ServiceSeq     INT = 0,  
@WorkingTag     NVARCHAR(10)= '',  
@CompanySeq     INT = 1,  
@LanguageSeq    INT = 1,  
@UserSeq  INT = 0,  
@PgmSeq   INT = 0  
AS  
DECLARE @docHandle    INT,    
@OU   NVARCHAR(200),   
@ADSearchKey    NVARCHAR(20)  
  
/*  
@ADSearchKey   
     == 전체조회  
2000447001 == AD HR계정 등록  
2000447002 == AD NAC계정 등록  
2000447003 == AD 비활성화  
2000447004 == AD 정보 수정  
*/  
  
EXEC sp_xml_preparedocument @docHandle OUTPUT, @xmlDocument  
  
SELECT  @ADSearchKey = ADSearchKey  
FROM OPENXML(@docHandle, N'/ROOT/DataBlock1', @xmlFlags)  
WITH (  
ADSearchKeyNVARCHAR(20)  
)  
  
IF EXISTS (SELECT * FROM Sysobjects where Name = '#yw_TADUsers_IF' AND xtype = 'U' )    
DROP TABLE #yw_TADUsers_IF    
CREATE TABLE #yw_TADUsers_IF    
(  
sAMAccountNameErp  NVARCHAR(40)   NOT NULL     
, snErp     NVARCHAR(200)   NOT NULL    
, givenNameErp   NVARCHAR(200)   NOT NULL    
, displayNameErp   NVARCHAR(200)   NOT NULL    
, departmentErp   NVARCHAR(200)   NOT NULL    
, titleErp    NVARCHAR(100)   NOT NULL    
, mailErp     NVARCHAR(100)   NOT NULL    
, mobileErp    NVARCHAR(40)   NOT NULL  
, homePhoneErp   NVARCHAR(200)   NOT NULL  
, CONSTRAINT PKyw_TADUsers_IF_TEMP PRIMARY KEY CLUSTERED (sAMAccountNameErp ASC)    
)
  
--0. 전체조회  
IF @ADSearchKey = N'' --1.   
BEGIN  
SELECT  
E.DeptName AS departmentErp  
, CASE WHEN D.EmpEngFirstName != N'' THEN CONCAT(D.EmpName, N'(', D.EmpEngLastName, D.EmpEngFirstName, N')') ELSE D.EmpName END AS displayNameErp  
, ISNULL(I.FullTelNo, N'') AS homePhoneErp--내선  
, D.EmpFamilyName AS snErp--성  
, F.MinorName AS titleErp --직위  
, ISNULL((SELECT TOP(1) PwdMailAdder FROM _TCAUser WHERE B.CompanySeq = CompanySeq AND A.EmpSeq = EmpSeq), N'' )AS mailErp
, Z.Cellphone AS mobileErp  
, A.EmpID AS sAMAccountNameErp     
, D.EmpFirstName AS givenNameErp  
, Z2.*  
FROM _fnAdmEmpOrdRetResidId(1, CONVERT(NCHAR(8), GETDATE(), 112)) AS A  
INNER JOIN _THRAdmOrdEmp AS B WITH(NOLOCK) ON B.IsOrdDateLast = '1' AND A.EmpSeq = B.EmpSeq  
AND B.OrdDate <= CASE WHEN ISNULL(A.RetireDate,'') > CONVERT(VARCHAR(8), GETDATE(), 112) THEN CONVERT(VARCHAR(8), GETDATE(), 112)  ELSE A.RetireDate END  
AND B.OrdEndDate >= CASE WHEN ISNULL(A.RetireDate,'') > CONVERT(VARCHAR(8), GETDATE(), 112) THEN CONVERT(VARCHAR(8), GETDATE(), 112)  ELSE A.RetireDate END  
INNER JOIN _TDAEmp AS D WITH(NOLOCK) ON B.CompanySeq = D.CompanySeq AND A.EmpSeq = D.EmpSeq  
INNER JOIN _TDADept AS E WITH(NOLOCK) ON B.DeptSeq = E.DeptSeq  
LEFT OUTER JOIN _TDAUMinor AS F WITH(NOLOCK) ON (B.CompanySeq = F.CompanySeq AND B.UMJpSeq = F.MinorSeq)-- 직위를 가져오기 위한 조인  
LEFT OUTER JOIN _TDAUMinor AS G WITH(NOLOCK) ON (B.CompanySeq = G.CompanySeq AND B.UMPgSeq = G.MinorSeq)-- 직급을 가져오기 위한 조인  
LEFT OUTER JOIN _TDAUMinor AS H WITH(NOLOCK) ON (B.CompanySeq = H.CompanySeq AND B.UMJdSeq    = H.MinorSeq)     -- 직책을 가져오기 위한 조인    
OUTER APPLY(    
SELECT TOP(1) I2.FullTelNo    
FROM yw_COTelNoUser I1 WITH(NOLOCK)    
INNER JOIN yw_COTelNo I2 WITH(NOLOCK) ON I1.CompanySeq = I2.CompanySeq and I1.TelNoSeq = I2.TelNoSeq    
WHERE I1.EmpSeq = a.EmpSeq    
ORDER BY I1.LastDateTime DESC  
) I   
INNER JOIN _TDAEmpIn AS Z WITH(NOLOCK) ON B.CompanySeq = Z.CompanySeq AND A.EmpId = Z.EmpId  
LEFT OUTER JOIN yw_TADUsers_IF AS Z2 WITH(NOLOCK) ON A.EmpID = Z2.sAMAccountName AND Z2.isDeleted = N'0'  
WHERE 1=1  
AND OrdName NOT IN(N'')  
AND RetDate = N'' 
  ORDER BY A.EmpID ASC  
END  
  
--1. AD HR계정 등록  
IF @ADSearchKey = N'2000447001' --1. 'AD HR계정 등록'  
BEGIN  
INSERT INTO #yw_TADUsers_IF (departmentErp, displayNameErp, homePhoneErp, snErp, titleErp, mailErp, mobileErp, sAMAccountNameErp, givenNameErp)     
SELECT  
E.DeptName AS departmentErp  
, CASE WHEN D.EmpEngFirstName != N'' THEN CONCAT(D.EmpName, N'(', D.EmpEngLastName, D.EmpEngFirstName, N')') ELSE D.EmpName END AS displayNameErp  
, ISNULL(I.FullTelNo, N'') AS homePhoneErp--내선    
, D.EmpFamilyName AS snErp--성  
, F.MinorName AS titleErp --직위  
, C.PwdMailAdder AS mailErp  
, A.Cellphone    AS mobileErp  
, A.EmpID AS sAMAccountNameErp  
, D.EmpFirstName AS givenNameErp  
FROM _TDAEmpIn AS A WITH(NOLOCK)  
INNER JOIN _THRAdmOrdEmp AS B WITH(NOLOCK) ON A.CompanySeq = B.CompanySeq AND B.IsOrdDateLast = '1' AND A.EmpSeq = B.EmpSeq  
AND B.OrdDate <= CASE WHEN ISNULL(A.RetireDate,'') > CONVERT(VARCHAR(8), GETDATE(), 112) THEN CONVERT(VARCHAR(8), GETDATE(), 112)  ELSE A.RetireDate END  
AND B.OrdEndDate >= CASE WHEN ISNULL(A.RetireDate,'') > CONVERT(VARCHAR(8), GETDATE(), 112) THEN CONVERT(VARCHAR(8), GETDATE(), 112)  ELSE A.RetireDate END  
INNER JOIN _TCAUser AS C WITH(NOLOCK) ON A.CompanySeq = C.CompanySeq AND A.EmpSeq = C.EmpSeq AND C.PwdMailAdder<> ''--PwdMailAdder(이메일)이 NULL이 아니어야 HR 계정이 있는 것임.  
INNER JOIN _TDAEmp AS D WITH(NOLOCK) ON A.CompanySeq = D.CompanySeq AND A.EmpSeq = D.EmpSeq
INNER JOIN _TDADept AS E WITH(NOLOCK) ON B.DeptSeq = E.DeptSeq  
LEFT OUTER JOIN _TDAUMinor AS F WITH(NOLOCK) ON (B.CompanySeq = F.CompanySeq AND B.UMJpSeq = F.MinorSeq)-- 직위를 가져오기 위한 조인  
LEFT OUTER JOIN _TDAUMinor AS G WITH(NOLOCK) ON (B.CompanySeq = G.CompanySeq AND B.UMPgSeq = G.MinorSeq)-- 직급을 가져오기 위한 조인  
LEFT OUTER JOIN _TDAUMinor AS H WITH(NOLOCK) ON (B.CompanySeq = H.CompanySeq AND B.UMJdSeq    = H.MinorSeq)     -- 직책을 가져오기 위한 조인    
OUTER APPLY(    
SELECT TOP(1) I2.FullTelNo    
FROM yw_COTelNoUser I1 WITH(NOLOCK)    
INNER JOIN yw_COTelNo I2 WITH(NOLOCK) ON I1.CompanySeq = I2.CompanySeq and I1.TelNoSeq = I2.TelNoSeq    
WHERE I1.EmpSeq = a.EmpSeq    
ORDER BY I1.LastDateTime DESC  
) I     
WHERE 1 = 1  
AND C.UserSeq NOT IN(1)--마스터 제외  
AND A.RetireDate >= CONVERT(VARCHAR(8), GETDATE(), 112)--현재기준 RetireDate가 없는 사원만 출력  
ORDER BY A.EmpID ASC  
DELETE A     
FROM  #yw_TADUsers_IF AS A     
INNER JOIN yw_TADUsers_IF AS B WITH(NOLOCK) ON A.sAMAccountNameErp = B.sAMAccountName    
WHERE 1=1    
  
SELECT A.*, B.*  
FROM #yw_TADUsers_IF AS A  
LEFT OUTER JOIN yw_TADUsers_IF AS B WITH(NOLOCK) ON A.sAMAccountNameErp = B.sAMAccountName  
ORDER BY sAMAccountName ASC    
TRUNCATE TABLE #yw_TADUsers_IF  
END  
  
--2. AD NAC계정 등록  
IF @ADSearchKey = N'2000447002' --2. 'AD NAC계정 등록'
--NAC 유저  
BEGIN  
INSERT INTO #yw_TADUsers_IF (departmentErp, displayNameErp, homePhoneErp, snErp, titleErp, mailErp, mobileErp, sAMAccountNameErp, givenNameErp) 
SELECT  
E.DeptName AS departmentErp  
, CASE WHEN D.EmpEngFirstName != N'' THEN CONCAT(D.EmpName, N'(', D.EmpEngLastName, D.EmpEngFirstName, N')') ELSE D.EmpName END AS displayNameErp  
, ISNULL(I.FullTelNo, N'') AS homePhoneErp--내선     
, D.EmpFamilyName AS snErp  
, F.MinorName AS titleErp --직위  
, N'' AS mailErp  
, Z.Cellphone AS mobileErp  
, A.EmpID AS sAMAccountNameErp  
, D.EmpFirstName AS givenNameErp  
FROM _fnAdmEmpOrdRetResidId(1, CONVERT(NCHAR(8), GETDATE(), 112)) AS A  
INNER JOIN _THRAdmOrdEmp AS B WITH(NOLOCK) ON B.IsOrdDateLast = '1' AND A.EmpSeq = B.EmpSeq  
AND B.OrdDate <= CASE WHEN ISNULL(A.RetireDate,'') > CONVERT(VARCHAR(8), GETDATE(), 112) THEN CONVERT(VARCHAR(8), GETDATE(), 112)  ELSE A.RetireDate END  
  AND B.OrdEndDate >= CASE WHEN ISNULL(A.RetireDate,'') > CONVERT(VARCHAR(8), GETDATE(), 112) THEN CONVERT(VARCHAR(8), GETDATE(), 112)  ELSE A.RetireDate END  
INNER JOIN _TDAEmp AS D WITH(NOLOCK) ON B.CompanySeq = D.CompanySeq AND A.EmpSeq = D.EmpSeq  
INNER JOIN _TDADept AS E WITH(NOLOCK) ON B.DeptSeq = E.DeptSeq  
LEFT OUTER JOIN _TDAUMinor AS F WITH(NOLOCK) ON (B.CompanySeq = F.CompanySeq AND B.UMJpSeq = F.MinorSeq)-- 직위를 가져오기 위한 조인  
LEFT OUTER JOIN _TDAUMinor AS G WITH(NOLOCK) ON (B.CompanySeq = G.CompanySeq AND B.UMPgSeq = G.MinorSeq)-- 직급을 가져오기 위한 조인  
LEFT OUTER JOIN _TDAUMinor AS H WITH(NOLOCK) ON (B.CompanySeq = H.CompanySeq AND B.UMJdSeq    = H.MinorSeq)     -- 직책을 가져오기 위한 조인    
OUTER APPLY(    
SELECT TOP(1) I2.FullTelNo    
FROM yw_COTelNoUser I1 WITH(NOLOCK)    
INNER JOIN yw_COTelNo I2 WITH(NOLOCK) ON I1.CompanySeq = I2.CompanySeq and I1.TelNoSeq = I2.TelNoSeq    
WHERE I1.EmpSeq = a.EmpSeq    
ORDER BY I1.LastDateTime DESC  
) I   
  
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
  
DELETE A     
FROM  #yw_TADUsers_IF AS A     
INNER JOIN yw_TADUsers_IF AS B WITH(NOLOCK) ON A.sAMAccountNameErp = B.sAMAccountName    
WHERE 1=1    
  
SELECT A.*, B.*  
FROM #yw_TADUsers_IF  AS A   
LEFT OUTER JOIN yw_TADUsers_IF AS B WITH(NOLOCK) ON A.sAMAccountNameErp = B.sAMAccountName  
ORDER BY sAMAccountNameErp ASC    
TRUNCATE TABLE #yw_TADUsers_IF    
END  
  
--3. AD 비활성화 대상  
IF @ADSearchKey = N'2000447003' --3. 'AD 비활성화'  
BEGIN  
SELECT   
N'' AS sAMAccountNameErp   
, N'' AS snErp
, N'' AS givenNameErp    
, N'' AS displayNameErp   
, N'' AS departmentErp    
, N'' AS titleErp     
, N'' AS mailErp     
, N'' AS homePhoneErp    
, A.*  
FROM yw_TADUsers_IF AS A WITH(NOLOCK)   
LEFT JOIN _TDAEmp AS B WITH(NOLOCK) ON A.sAMAccountName = B.Empid  
WHERE 1=1  
AND B.EMPID IS NULL  
END  
  
--4. AD정보수정대상  
IF @ADSearchKey = N'2000447004' --4. 'AD 정보 수정'  
BEGIN  
INSERT INTO #yw_TADUsers_IF (departmentErp, displayNameErp, homePhoneErp, snErp, titleErp, mailErp, mobileErp, sAMAccountNameErp, givenNameErp)     
SELECT  
E.DeptName AS departmentErp  
, CASE WHEN D.EmpEngFirstName != N'' THEN CONCAT(D.EmpName, N'(', D.EmpEngLastName, D.EmpEngFirstName, N')') ELSE D.EmpName END AS displayNameErp  
, ISNULL(I.FullTelNo, N'') AS homePhoneErp--내선  
, D.EmpFamilyName AS snErp--성  
, F.MinorName AS titleErp --직위  
, ISNULL((SELECT TOP(1) PwdMailAdder FROM _TCAUser WHERE B.CompanySeq = CompanySeq AND A.EmpSeq = EmpSeq), N'' )AS mailErp
, Z.Cellphone AS mobileErp  
, A.EmpID AS sAMAccountNameErp     
, D.EmpFirstName AS givenNameErp  
FROM _fnAdmEmpOrdRetResidId(1, CONVERT(NCHAR(8), GETDATE(), 112)) AS A  
INNER JOIN _THRAdmOrdEmp AS B WITH(NOLOCK) ON B.IsOrdDateLast = '1' AND A.EmpSeq = B.EmpSeq  
AND B.OrdDate <= CASE WHEN ISNULL(A.RetireDate,'') > CONVERT(VARCHAR(8), GETDATE(), 112) THEN CONVERT(VARCHAR(8), GETDATE(), 112)  ELSE A.RetireDate END  
  AND B.OrdEndDate >= CASE WHEN ISNULL(A.RetireDate,'') > CONVERT(VARCHAR(8), GETDATE(), 112) THEN CONVERT(VARCHAR(8), GETDATE(), 112)  ELSE A.RetireDate END  
INNER JOIN _TDAEmp AS D WITH(NOLOCK) ON B.CompanySeq = D.CompanySeq AND A.EmpSeq = D.EmpSeq  
INNER JOIN _TDADept AS E WITH(NOLOCK) ON B.DeptSeq = E.DeptSeq  
LEFT OUTER JOIN _TDAUMinor AS F WITH(NOLOCK) ON (B.CompanySeq = F.CompanySeq AND B.UMJpSeq = F.MinorSeq)-- 직위를 가져오기 위한 조인  
LEFT OUTER JOIN _TDAUMinor AS G WITH(NOLOCK) ON (B.CompanySeq = G.CompanySeq AND B.UMPgSeq = G.MinorSeq)-- 직급을 가져오기 위한 조인  
LEFT OUTER JOIN _TDAUMinor AS H WITH(NOLOCK) ON (B.CompanySeq = H.CompanySeq AND B.UMJdSeq    = H.MinorSeq)     -- 직책을 가져오기 위한 조인    
OUTER APPLY(    
SELECT TOP(1) I2.FullTelNo    
FROM yw_COTelNoUser I1 WITH(NOLOCK)    
INNER JOIN yw_COTelNo I2 WITH(NOLOCK) ON I1.CompanySeq = I2.CompanySeq and I1.TelNoSeq = I2.TelNoSeq    
WHERE I1.EmpSeq = a.EmpSeq    
ORDER BY I1.LastDateTime DESC  
) I   
INNER JOIN _TDAEmpIn AS Z WITH(NOLOCK) ON B.CompanySeq = Z.CompanySeq AND A.EmpId = Z.EmpId  
INNER JOIN yw_TADUsers_IF AS Z2 WITH(NOLOCK) ON A.EmpID = Z2.sAMAccountName AND Z2.isDeleted = N'0'  
WHERE 1=1  
AND OrdName NOT IN(N'')  
AND RetDate = N'' 
ORDER BY A.EmpID ASC  
  
SELECT *   
FROM (  
SELECT   
A.sAMAccountNameErp  
, (CASE WHEN A.snErp = B.sn THEN N'' ELSE A.snErp END) AS snErp  
, (CASE WHEN A.givenNameErp = B.givenName THEN N'' ELSE A.givenNameErp END) AS givenNameErp  
, (CASE WHEN A.displayNameErp = B.displayName THEN N'' ELSE A.displayNameErp END) AS displayNameErp  
, (CASE WHEN A.departmentErp = B.department THEN N'' ELSE A.departmentErp END) AS departmentErp  
, (CASE WHEN A.titleErp = B.title THEN N'' ELSE A.titleErp END) AS titleErp  
, (CASE WHEN A.mailErp = B.mail THEN N'' ELSE A.mailErp END) AS mailErp  
, (CASE WHEN A.mobileErp = B.mobile THEN N'' ELSE A.mobileErp END) AS mobileErp  
, (CASE WHEN A.homePhoneErp = B.homePhone THEN N'' ELSE A.homePhoneErp END) AS homePhoneErp  
, B.*  
FROM #yw_TADUsers_IF AS A --ERP정보  
LEFT OUTER JOIN yw_TADUsers_IF AS B WITH(NOLOCK) ON A.sAMAccountNameErp = B.sAMAccountName --AD정보  
) AS A   
WHERE 1=1   
AND (A.snErp != N''  
OR A.givenNameErp != N''  
OR A.displayNameErp != N''  
OR A.departmentErp != N''  
OR A.titleErp != N''  
OR A.mailErp != N''  
OR A.mobileErp != N''  
OR A.homePhoneErp != N'')  
END  
  
RETURN     