SELECT
-----Failure rate per algo per world-----
--Random, World1
(SELECT CAST(FAILED as float) / TOTAL FROM( SELECT
(SELECT COUNT(*) FROM Results WHERE Algorithm = 'Random' AND World = 'World1' AND Completable = 0) AS FAILED,
(SELECT COUNT(*) FROM Results WHERE Algorithm = 'Random' AND World = 'World1') AS TOTAL)) AS R1F,
--Random, World2
(SELECT CAST(FAILED as float) / TOTAL FROM( SELECT
(SELECT COUNT(*) FROM Results WHERE Algorithm = 'Random' AND World = 'World2' AND Completable = 0) AS FAILED,
(SELECT COUNT(*) FROM Results WHERE Algorithm = 'Random' AND World = 'World2') AS TOTAL)) AS R2F,
--Random, World3
(SELECT CAST(FAILED as float) / TOTAL FROM( SELECT
(SELECT COUNT(*) FROM Results WHERE Algorithm = 'Random' AND World = 'World3' AND Completable = 0) AS FAILED,
(SELECT COUNT(*) FROM Results WHERE Algorithm = 'Random' AND World = 'World3') AS TOTAL)) AS R3F,
--Random, World4
(SELECT CAST(FAILED as float) / TOTAL FROM( SELECT
(SELECT COUNT(*) FROM Results WHERE Algorithm = 'Random' AND World = 'World4' AND Completable = 0) AS FAILED,
(SELECT COUNT(*) FROM Results WHERE Algorithm = 'Random' AND World = 'World4') AS TOTAL)) AS R4F,
--Random, World5
(SELECT CAST(FAILED as float) / TOTAL FROM( SELECT
(SELECT COUNT(*) FROM Results WHERE Algorithm = 'Random' AND World = 'World5' AND Completable = 0) AS FAILED,
(SELECT COUNT(*) FROM Results WHERE Algorithm = 'Random' AND World = 'World5') AS TOTAL)) AS R5F,

--Forward, World1
(SELECT CAST(FAILED as float) / TOTAL FROM( SELECT
(SELECT COUNT(*) FROM Results WHERE Algorithm = 'Forward' AND World = 'World1' AND Completable = 0) AS FAILED,
(SELECT COUNT(*) FROM Results WHERE Algorithm = 'Forward' AND World = 'World1') AS TOTAL)) AS F1F,
--Forward, World2
(SELECT CAST(FAILED as float) / TOTAL FROM( SELECT
(SELECT COUNT(*) FROM Results WHERE Algorithm = 'Forward' AND World = 'World2' AND Completable = 0) AS FAILED,
(SELECT COUNT(*) FROM Results WHERE Algorithm = 'Forward' AND World = 'World2') AS TOTAL)) AS F2F,
--Forward, World3
(SELECT CAST(FAILED as float) / TOTAL FROM( SELECT
(SELECT COUNT(*) FROM Results WHERE Algorithm = 'Forward' AND World = 'World3' AND Completable = 0) AS FAILED,
(SELECT COUNT(*) FROM Results WHERE Algorithm = 'Forward' AND World = 'World3') AS TOTAL)) AS F3F,
--Forward, World4
(SELECT CAST(FAILED as float) / TOTAL FROM( SELECT
(SELECT COUNT(*) FROM Results WHERE Algorithm = 'Forward' AND World = 'World4' AND Completable = 0) AS FAILED,
(SELECT COUNT(*) FROM Results WHERE Algorithm = 'Forward' AND World = 'World4') AS TOTAL)) AS F4F,
--Forward, World5
(SELECT CAST(FAILED as float) / TOTAL FROM( SELECT
(SELECT COUNT(*) FROM Results WHERE Algorithm = 'Forward' AND World = 'World5' AND Completable = 0) AS FAILED,
(SELECT COUNT(*) FROM Results WHERE Algorithm = 'Forward' AND World = 'World5') AS TOTAL)) AS F5F,

--Assumed, World1
(SELECT CAST(FAILED as float) / TOTAL FROM( SELECT
(SELECT COUNT(*) FROM Results WHERE Algorithm = 'Assumed' AND World = 'World1' AND Completable = 0) AS FAILED,
(SELECT COUNT(*) FROM Results WHERE Algorithm = 'Assumed' AND World = 'World1') AS TOTAL)) AS A1F,
--Assumed, World2
(SELECT CAST(FAILED as float) / TOTAL FROM( SELECT
(SELECT COUNT(*) FROM Results WHERE Algorithm = 'Assumed' AND World = 'World2' AND Completable = 0) AS FAILED,
(SELECT COUNT(*) FROM Results WHERE Algorithm = 'Assumed' AND World = 'World2') AS TOTAL)) AS A2F,
--Assumed, World3
(SELECT CAST(FAILED as float) / TOTAL FROM( SELECT
(SELECT COUNT(*) FROM Results WHERE Algorithm = 'Assumed' AND World = 'World3' AND Completable = 0) AS FAILED,
(SELECT COUNT(*) FROM Results WHERE Algorithm = 'Assumed' AND World = 'World3') AS TOTAL)) AS A3F,
--Assumed, World4
(SELECT CAST(FAILED as float) / TOTAL FROM( SELECT
(SELECT COUNT(*) FROM Results WHERE Algorithm = 'Assumed' AND World = 'World4' AND Completable = 0) AS FAILED,
(SELECT COUNT(*) FROM Results WHERE Algorithm = 'Assumed' AND World = 'World4') AS TOTAL)) AS A4F,
--Assumed, World5
(SELECT CAST(FAILED as float) / TOTAL FROM( SELECT
(SELECT COUNT(*) FROM Results WHERE Algorithm = 'Assumed' AND World = 'World5' AND Completable = 0) AS FAILED,
(SELECT COUNT(*) FROM Results WHERE Algorithm = 'Assumed' AND World = 'World5') AS TOTAL)) AS A5F,

-----Average Execution Time Per Algo Per World-----
--Random, World1
(SELECT AVG(ExecutionTime) FROM Results WHERE Algorithm = 'Random' AND World = 'World1') AS R1E,
--Random, World2
(SELECT AVG(ExecutionTime) FROM Results WHERE Algorithm = 'Random' AND World = 'World2') AS R2E,
--Random, World3
(SELECT AVG(ExecutionTime) FROM Results WHERE Algorithm = 'Random' AND World = 'World3') AS R3E,
--Random, World4
(SELECT AVG(ExecutionTime) FROM Results WHERE Algorithm = 'Random' AND World = 'World4') AS R4E,
--Random, World5
(SELECT AVG(ExecutionTime) FROM Results WHERE Algorithm = 'Random' AND World = 'World5') AS R5E,

--Forward, World1
(SELECT AVG(ExecutionTime) FROM Results WHERE Algorithm = 'Forward' AND World = 'World1') AS F1E,
--Forward, World2
(SELECT AVG(ExecutionTime) FROM Results WHERE Algorithm = 'Forward' AND World = 'World2') AS F2E,
--Forward, World3
(SELECT AVG(ExecutionTime) FROM Results WHERE Algorithm = 'Forward' AND World = 'World3') AS F3E,
--Forward, World4
(SELECT AVG(ExecutionTime) FROM Results WHERE Algorithm = 'Forward' AND World = 'World4') AS F4E,
--Forward, World5
(SELECT AVG(ExecutionTime) FROM Results WHERE Algorithm = 'Forward' AND World = 'World5') AS F5E,

--Assumed, World1
(SELECT AVG(ExecutionTime) FROM Results WHERE Algorithm = 'Assumed' AND World = 'World1') AS A1E,
--Assumed, World2
(SELECT AVG(ExecutionTime) FROM Results WHERE Algorithm = 'Assumed' AND World = 'World2') AS A2E,
--Assumed, World3
(SELECT AVG(ExecutionTime) FROM Results WHERE Algorithm = 'Assumed' AND World = 'World3') AS A3E,
--Assumed, World4
(SELECT AVG(ExecutionTime) FROM Results WHERE Algorithm = 'Assumed' AND World = 'World4') AS A4E,
--Assumed, World5
(SELECT AVG(ExecutionTime) FROM Results WHERE Algorithm = 'Assumed' AND World = 'World5') AS A5E,

-----Average Bias Per Algo Per World-----
--Random, World1
(SELECT AVG(Bias) FROM Results WHERE Algorithm = 'Random' AND World = 'World1') AS R1B,
--Random, World2
(SELECT AVG(Bias) FROM Results WHERE Algorithm = 'Random' AND World = 'World2') AS R2B,
--Random, World3
(SELECT AVG(Bias) FROM Results WHERE Algorithm = 'Random' AND World = 'World3') AS R3B,
--Random, World4
(SELECT AVG(Bias) FROM Results WHERE Algorithm = 'Random' AND World = 'World4') AS R4B,
--Random, World5
(SELECT AVG(Bias) FROM Results WHERE Algorithm = 'Random' AND World = 'World5') AS R5B,

--Random Successful, World1
(SELECT AVG(Bias) FROM Results WHERE Algorithm = 'Random' AND World = 'World1' AND Completable = 1) AS RS1B,
--Random Successful, World2
(SELECT AVG(Bias) FROM Results WHERE Algorithm = 'Random' AND World = 'World2' AND Completable = 1) AS RS2B,
--Random Successful, World3
(SELECT AVG(Bias) FROM Results WHERE Algorithm = 'Random' AND World = 'World3' AND Completable = 1) AS RS3B,
--Random Successful, World4
(SELECT AVG(Bias) FROM Results WHERE Algorithm = 'Random' AND World = 'World4' AND Completable = 1) AS RS4B,
--Random Successful, World5
(SELECT AVG(Bias) FROM Results WHERE Algorithm = 'Random' AND World = 'World5' AND Completable = 1) AS RS5B,

--Forward, World1
(SELECT AVG(Bias) FROM Results WHERE Algorithm = 'Forward' AND World = 'World1') AS F1B,
--Forward, World2
(SELECT AVG(Bias) FROM Results WHERE Algorithm = 'Forward' AND World = 'World2') AS F2B,
--Forward, World3
(SELECT AVG(Bias) FROM Results WHERE Algorithm = 'Forward' AND World = 'World3') AS F3B,
--Forward, World4
(SELECT AVG(Bias) FROM Results WHERE Algorithm = 'Forward' AND World = 'World4') AS F4B,
--Forward, World5
(SELECT AVG(Bias) FROM Results WHERE Algorithm = 'Forward' AND World = 'World5') AS F5B,

--Assumed, World1
(SELECT AVG(Bias) FROM Results WHERE Algorithm = 'Assumed' AND World = 'World1') AS A1B,
--Assumed, World2
(SELECT AVG(Bias) FROM Results WHERE Algorithm = 'Assumed' AND World = 'World2') AS A2B,
--Assumed, World3
(SELECT AVG(Bias) FROM Results WHERE Algorithm = 'Assumed' AND World = 'World3') AS A3B,
--Assumed, World4
(SELECT AVG(Bias) FROM Results WHERE Algorithm = 'Assumed' AND World = 'World4') AS A4B,
--Assumed, World5
(SELECT AVG(Bias) FROM Results WHERE Algorithm = 'Assumed' AND World = 'World5') AS A5B,

-----Average Interestingness Per Algo Per World-----
--Random, World1
(SELECT AVG(Interestingness) FROM Results WHERE Algorithm = 'Random' AND World = 'World1') AS R1I,
--Random, World2
(SELECT AVG(Interestingness) FROM Results WHERE Algorithm = 'Random' AND World = 'World2') AS R2I,
--Random, World3
(SELECT AVG(Interestingness) FROM Results WHERE Algorithm = 'Random' AND World = 'World3') AS R3I,
--Random, World4
(SELECT AVG(Interestingness) FROM Results WHERE Algorithm = 'Random' AND World = 'World4') AS R4I,
--Random, World5
(SELECT AVG(Interestingness) FROM Results WHERE Algorithm = 'Random' AND World = 'World5') AS R5I,

--Random Successful, World1
(SELECT AVG(Interestingness) FROM Results WHERE Algorithm = 'Random' AND World = 'World1' AND Completable = 1) AS RS1I,
--Random Successful, World2
(SELECT AVG(Interestingness) FROM Results WHERE Algorithm = 'Random' AND World = 'World2' AND Completable = 1) AS RS2I,
--Random Successful, World3
(SELECT AVG(Interestingness) FROM Results WHERE Algorithm = 'Random' AND World = 'World3' AND Completable = 1) AS RS3I,
--Random Successful, World4
(SELECT AVG(Interestingness) FROM Results WHERE Algorithm = 'Random' AND World = 'World4' AND Completable = 1) AS RS4I,
--Random Successful, World5
(SELECT AVG(Interestingness) FROM Results WHERE Algorithm = 'Random' AND World = 'World5' AND Completable = 1) AS RS5I,

--Forward, World1
(SELECT AVG(Interestingness) FROM Results WHERE Algorithm = 'Forward' AND World = 'World1') AS F1I,
--Forward, World2
(SELECT AVG(Interestingness) FROM Results WHERE Algorithm = 'Forward' AND World = 'World2') AS F2I,
--Forward, World3
(SELECT AVG(Interestingness) FROM Results WHERE Algorithm = 'Forward' AND World = 'World3') AS F3I,
--Forward, World4
(SELECT AVG(Interestingness) FROM Results WHERE Algorithm = 'Forward' AND World = 'World4') AS F4I,
--Forward, World5
(SELECT AVG(Interestingness) FROM Results WHERE Algorithm = 'Forward' AND World = 'World5') AS F5I,

--Assumed, World1
(SELECT AVG(Interestingness) FROM Results WHERE Algorithm = 'Assumed' AND World = 'World1') AS A1I,
--Assumed, World2
(SELECT AVG(Interestingness) FROM Results WHERE Algorithm = 'Assumed' AND World = 'World2') AS A2I,
--Assumed, World3
(SELECT AVG(Interestingness) FROM Results WHERE Algorithm = 'Assumed' AND World = 'World3') AS A3I,
--Assumed, World4
(SELECT AVG(Interestingness) FROM Results WHERE Algorithm = 'Assumed' AND World = 'World4') AS A4I,
--Assumed, World5
(SELECT AVG(Interestingness) FROM Results WHERE Algorithm = 'Assumed' AND World = 'World5') AS A5I
