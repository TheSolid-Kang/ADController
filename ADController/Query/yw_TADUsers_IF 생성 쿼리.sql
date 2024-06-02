IF EXISTS (SELECT * FROM Sysobjects where Name = 'yw_TADUsers_IF' AND xtype = 'U' )
    Drop table yw_TADUsers_IF

CREATE TABLE yw_TADUsers_IF
(
    sAMAccountName		NVARCHAR(40) 	 NOT NULL, 
    cn		NVARCHAR(200) 	 NOT NULL, 
    description		NVARCHAR(400) 	 NOT NULL, 
    department		NVARCHAR(200) 	 NOT NULL, 
    displayName		NVARCHAR(200) 	 NOT NULL, 
    distinguishedName		NVARCHAR(400) 	 NOT NULL, 
    givenName		NVARCHAR(200) 	 NOT NULL, 
    homePhone		NVARCHAR(200) 	 NOT NULL, 
    lastLogon		DATETIME 	 NOT NULL, 
    otherHomePhone		NVARCHAR(200) 	 NOT NULL, 
    otherMobile		NVARCHAR(200) 	 NOT NULL, 
    pwdLastSet		DATETIME 	 NOT NULL, 
    sn		NVARCHAR(200) 	 NOT NULL, 
    title		NVARCHAR(100) 	 NOT NULL, 
    mail		NVARCHAR(100) 	 NOT NULL, 
    manager		NVARCHAR(200) 	 NOT NULL, 
    userPrincipalName		NVARCHAR(200) 	 NOT NULL, 
    uSNChanged		INT 	 NOT NULL, 
    uSNCreated		INT 	 NOT NULL, 
    whenChanged		DATETIME 	 NOT NULL, 
    whenCreated		DATETIME 	 NOT NULL, 
    condition		NVARCHAR(20) 	 NULL

CONSTRAINT PKyw_TADUsers_IF PRIMARY KEY CLUSTERED (sAMAccountName ASC)

) 


IF EXISTS (SELECT * FROM Sysobjects where Name = 'yw_TADUsers_IFLog' AND xtype = 'U' )
    Drop table yw_TADUsers_IFLog

CREATE TABLE yw_TADUsers_IFLog
(
    LogSeq		INT IDENTITY(1,1) NOT NULL, 
    LogUserSeq		INT NOT NULL, 
    LogDateTime		DATETIME NOT NULL, 
    LogType		NCHAR(1) NOT NULL, 
    LogPgmSeq		INT NULL, 
    sAMAccountName		NVARCHAR(40) 	 NOT NULL, 
    cn		NVARCHAR(200) 	 NOT NULL, 
    description		NVARCHAR(400) 	 NOT NULL, 
    department		NVARCHAR(200) 	 NOT NULL, 
    displayName		NVARCHAR(200) 	 NOT NULL, 
    distinguishedName		NVARCHAR(400) 	 NOT NULL, 
    givenName		NVARCHAR(200) 	 NOT NULL, 
    homePhone		NVARCHAR(200) 	 NOT NULL, 
    lastLogon		DATETIME 	 NOT NULL, 
    otherHomePhone		NVARCHAR(200) 	 NOT NULL, 
    otherMobile		NVARCHAR(200) 	 NOT NULL, 
    pwdLastSet		DATETIME 	 NOT NULL, 
    sn		NVARCHAR(200) 	 NOT NULL, 
    title		NVARCHAR(100) 	 NOT NULL, 
    mail		NVARCHAR(100) 	 NOT NULL, 
    manager		NVARCHAR(200) 	 NOT NULL, 
    userPrincipalName		NVARCHAR(200) 	 NOT NULL, 
    uSNChanged		INT 	 NOT NULL, 
    uSNCreated		INT 	 NOT NULL, 
    whenChanged		DATETIME 	 NOT NULL, 
    whenCreated		DATETIME 	 NOT NULL, 
    condition		NVARCHAR(20) 	 NULL
)

CREATE UNIQUE CLUSTERED INDEX IDXTempyw_TADUsers_IFLog ON yw_TADUsers_IFLog (LogSeq)
