ALTER PROC IS_SelAppLog_All
    @CategoryId	INT = NULL,
    @MachineId INT = NULL,
    @Severity NVARCHAR(300) = NULL,
    @IsProduction BIT = NULL,
    @ClientTimeZone INT,
    @FromDate DATETIME,
    @ToDate DATETIME
AS
/*
Created: 20151021@Larry.Tran
Task: Get AppLog_Log by criteria. 
DB: bodb_CustomerLog
Original: 

Revisions:
    
Param's Explanation:
    - @Severity = 'Error, Critical': support to filter multiple severities
    - @ClientTimeZone: The client time zone.

*/
BEGIN
    SET NOCOUNT ON

    --Convert to UTC+0
    SET @FromDate = DATEADD(hour,@ClientTimeZone*-1,@FromDate)
    SET @ToDate = DATEADD(hour,@ClientTimeZone*-1,@ToDate)

    SELECT DISTINCT
        lg.LogID AS LogId, 
        DATEADD(hour,@ClientTimeZone, lg.[Timestamp]) AS [Timestamp], --Convert to client time
        lg.Severity,
        lg.FormattedMessage,
        cat.CategoryID AS CategoryId,
        cat.CategoryName,
        m.MachineId,
        m.MachineName,
        m.MachineIP AS MachineIp,
        m.IsProduction
    FROM dbo.AppLog_Log AS lg WITH (NOLOCK)
        INNER JOIN  dbo.AppLog_CategoryLog AS catLog WITH (NOLOCK)
            ON catLog.LogID = lg.LogID
        INNER JOIN dbo.AppLog_Category AS cat WITH (NOLOCK)
            ON cat.CategoryID = catLog.CategoryID
        INNER JOIN dbo.AppLog_Machine AS m WITH (NOLOCK)
            ON m.MachineId = lg.MachineId
    WHERE lg.[Timestamp] BETWEEN @FromDate AND @ToDate
        AND (@CategoryId IS NULL OR catLog.CategoryID = @CategoryId)
        AND (@MachineId IS NULL OR m.MachineId = @MachineId)
        AND (
            @Severity IS NULL 
            OR lg.Severity IN (SELECT val FROM dbo.StrSplit(@Severity,','))
        )
        AND (@IsProduction IS NULL OR m.IsProduction = @IsProduction)
    ORDER BY [Timestamp]
END