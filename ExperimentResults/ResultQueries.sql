SELECT
-----Failure rate per algo per world-----
--Random, World1
(SELECT CAST(FAILED as float) / TOTAL FROM( SELECT
(SELECT COUNT(*) FROM Results WHERE Algorithm = 'Random' AND World = 'World1' AND Completable = 0) AS FAILED,
(SELECT COUNT(*) FROM Results WHERE Algorithm = 'Random' AND World = 'World1') AS TOTAL)) AS R1R,
--Random, World2
(SELECT CAST(FAILED as float) / TOTAL FROM( SELECT
(SELECT COUNT(*) FROM Results WHERE Algorithm = 'Random' AND World = 'World2' AND Completable = 0) AS FAILED,
(SELECT COUNT(*) FROM Results WHERE Algorithm = 'Random' AND World = 'World2') AS TOTAL)) AS R2R,
--Random, World3
(SELECT CAST(FAILED as float) / TOTAL FROM( SELECT
(SELECT COUNT(*) FROM Results WHERE Algorithm = 'Random' AND World = 'World3' AND Completable = 0) AS FAILED,
(SELECT COUNT(*) FROM Results WHERE Algorithm = 'Random' AND World = 'World3') AS TOTAL)) AS R3R,
--Random, World4
(SELECT CAST(FAILED as float) / TOTAL FROM( SELECT
(SELECT COUNT(*) FROM Results WHERE Algorithm = 'Random' AND World = 'World4' AND Completable = 0) AS FAILED,
(SELECT COUNT(*) FROM Results WHERE Algorithm = 'Random' AND World = 'World4') AS TOTAL)) AS R4R,
--Random, World5
(SELECT CAST(FAILED as float) / TOTAL FROM( SELECT
(SELECT COUNT(*) FROM Results WHERE Algorithm = 'Random' AND World = 'World5' AND Completable = 0) AS FAILED,
(SELECT COUNT(*) FROM Results WHERE Algorithm = 'Random' AND World = 'World5') AS TOTAL)) AS R5R,

--Forward, World1
(SELECT CAST(FAILED as float) / TOTAL FROM( SELECT
(SELECT COUNT(*) FROM Results WHERE Algorithm = 'Forward' AND World = 'World1' AND Completable = 0) AS FAILED,
(SELECT COUNT(*) FROM Results WHERE Algorithm = 'Forward' AND World = 'World1') AS TOTAL)) AS F1R,
--Forward, World2
(SELECT CAST(FAILED as float) / TOTAL FROM( SELECT
(SELECT COUNT(*) FROM Results WHERE Algorithm = 'Forward' AND World = 'World2' AND Completable = 0) AS FAILED,
(SELECT COUNT(*) FROM Results WHERE Algorithm = 'Forward' AND World = 'World2') AS TOTAL)) AS F2R,
--Forward, World3
(SELECT CAST(FAILED as float) / TOTAL FROM( SELECT
(SELECT COUNT(*) FROM Results WHERE Algorithm = 'Forward' AND World = 'World3' AND Completable = 0) AS FAILED,
(SELECT COUNT(*) FROM Results WHERE Algorithm = 'Forward' AND World = 'World3') AS TOTAL)) AS F3R,
--Forward, World4
(SELECT CAST(FAILED as float) / TOTAL FROM( SELECT
(SELECT COUNT(*) FROM Results WHERE Algorithm = 'Forward' AND World = 'World4' AND Completable = 0) AS FAILED,
(SELECT COUNT(*) FROM Results WHERE Algorithm = 'Forward' AND World = 'World4') AS TOTAL)) AS F4R,
--Forward, World5
(SELECT CAST(FAILED as float) / TOTAL FROM( SELECT
(SELECT COUNT(*) FROM Results WHERE Algorithm = 'Forward' AND World = 'World5' AND Completable = 0) AS FAILED,
(SELECT COUNT(*) FROM Results WHERE Algorithm = 'Forward' AND World = 'World5') AS TOTAL)) AS F5R,

--Assumed, World1
(SELECT CAST(FAILED as float) / TOTAL FROM( SELECT
(SELECT COUNT(*) FROM Results WHERE Algorithm = 'Assumed' AND World = 'World1' AND Completable = 0) AS FAILED,
(SELECT COUNT(*) FROM Results WHERE Algorithm = 'Assumed' AND World = 'World1') AS TOTAL)) AS A1R,
--Assumed, World2
(SELECT CAST(FAILED as float) / TOTAL FROM( SELECT
(SELECT COUNT(*) FROM Results WHERE Algorithm = 'Assumed' AND World = 'World2' AND Completable = 0) AS FAILED,
(SELECT COUNT(*) FROM Results WHERE Algorithm = 'Assumed' AND World = 'World2') AS TOTAL)) AS A2R,
--Assumed, World3
(SELECT CAST(FAILED as float) / TOTAL FROM( SELECT
(SELECT COUNT(*) FROM Results WHERE Algorithm = 'Assumed' AND World = 'World3' AND Completable = 0) AS FAILED,
(SELECT COUNT(*) FROM Results WHERE Algorithm = 'Assumed' AND World = 'World3') AS TOTAL)) AS A3R,
--Assumed, World4
(SELECT CAST(FAILED as float) / TOTAL FROM( SELECT
(SELECT COUNT(*) FROM Results WHERE Algorithm = 'Assumed' AND World = 'World4' AND Completable = 0) AS FAILED,
(SELECT COUNT(*) FROM Results WHERE Algorithm = 'Assumed' AND World = 'World4') AS TOTAL)) AS A4R,
--Assumed, World5
(SELECT CAST(FAILED as float) / TOTAL FROM( SELECT
(SELECT COUNT(*) FROM Results WHERE Algorithm = 'Assumed' AND World = 'World5' AND Completable = 0) AS FAILED,
(SELECT COUNT(*) FROM Results WHERE Algorithm = 'Assumed' AND World = 'World5') AS TOTAL)) AS A5R,

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


--For the following metrics, we only consider successful results


-----Average Bias Per Algo Per World-----
--Random, World1
(SELECT AVG(Bias) FROM Results WHERE Algorithm = 'Random' AND World = 'World1' AND Completable = 1) AS R1B,
--Random, World2
(SELECT AVG(Bias) FROM Results WHERE Algorithm = 'Random' AND World = 'World2' AND Completable = 1) AS R2B,
--Random, World3
(SELECT AVG(Bias) FROM Results WHERE Algorithm = 'Random' AND World = 'World3' AND Completable = 1) AS R3B,
--Random, World4
(SELECT AVG(Bias) FROM Results WHERE Algorithm = 'Random' AND World = 'World4' AND Completable = 1) AS R4B,
--Random, World5
(SELECT AVG(Bias) FROM Results WHERE Algorithm = 'Random' AND World = 'World5' AND Completable = 1) AS R5B,

--Forward, World1
(SELECT AVG(Bias) FROM Results WHERE Algorithm = 'Forward' AND World = 'World1' AND Completable = 1) AS F1B,
--Forward, World2
(SELECT AVG(Bias) FROM Results WHERE Algorithm = 'Forward' AND World = 'World2' AND Completable = 1) AS F2B,
--Forward, World3
(SELECT AVG(Bias) FROM Results WHERE Algorithm = 'Forward' AND World = 'World3' AND Completable = 1) AS F3B,
--Forward, World4
(SELECT AVG(Bias) FROM Results WHERE Algorithm = 'Forward' AND World = 'World4' AND Completable = 1) AS F4B,
--Forward, World5
(SELECT AVG(Bias) FROM Results WHERE Algorithm = 'Forward' AND World = 'World5' AND Completable = 1) AS F5B,

--Assumed, World1
(SELECT AVG(Bias) FROM Results WHERE Algorithm = 'Assumed' AND World = 'World1' AND Completable = 1) AS A1B,
--Assumed, World2
(SELECT AVG(Bias) FROM Results WHERE Algorithm = 'Assumed' AND World = 'World2' AND Completable = 1) AS A2B,
--Assumed, World3
(SELECT AVG(Bias) FROM Results WHERE Algorithm = 'Assumed' AND World = 'World3' AND Completable = 1) AS A3B,
--Assumed, World4
(SELECT AVG(Bias) FROM Results WHERE Algorithm = 'Assumed' AND World = 'World4' AND Completable = 1) AS A4B,
--Assumed, World5
(SELECT AVG(Bias) FROM Results WHERE Algorithm = 'Assumed' AND World = 'World5' AND Completable = 1) AS A5B,

-----Percentage of times bias is toward end per algo per world-----
--Random, World1
(SELECT CAST(TOWARDEND as float) / TOTAL FROM( SELECT
(SELECT COUNT(*) FROM Results WHERE Algorithm = 'Random' AND World = 'World1' AND BiasDirection = 1 AND Completable = 1) AS TOWARDEND,
(SELECT COUNT(*) FROM Results WHERE Algorithm = 'Random' AND World = 'World1' AND Completable = 1) AS TOTAL)) AS R1Bd,
--Random, World2
(SELECT CAST(TOWARDEND as float) / TOTAL FROM( SELECT
(SELECT COUNT(*) FROM Results WHERE Algorithm = 'Random' AND World = 'World2' AND BiasDirection = 1 AND Completable = 1) AS TOWARDEND,
(SELECT COUNT(*) FROM Results WHERE Algorithm = 'Random' AND World = 'World2' AND Completable = 1) AS TOTAL)) AS R2Bd,
--Random, World3
(SELECT CAST(TOWARDEND as float) / TOTAL FROM( SELECT
(SELECT COUNT(*) FROM Results WHERE Algorithm = 'Random' AND World = 'World3' AND BiasDirection = 1 AND Completable = 1) AS TOWARDEND,
(SELECT COUNT(*) FROM Results WHERE Algorithm = 'Random' AND World = 'World3' AND Completable = 1) AS TOTAL)) AS R3Bd,
--Random, World4
(SELECT CAST(TOWARDEND as float) / TOTAL FROM( SELECT
(SELECT COUNT(*) FROM Results WHERE Algorithm = 'Random' AND World = 'World4' AND BiasDirection = 1 AND Completable = 1) AS TOWARDEND,
(SELECT COUNT(*) FROM Results WHERE Algorithm = 'Random' AND World = 'World4' AND Completable = 1) AS TOTAL)) AS R4Bd,
--Random, World5
(SELECT CAST(TOWARDEND as float) / TOTAL FROM( SELECT
(SELECT COUNT(*) FROM Results WHERE Algorithm = 'Random' AND World = 'World5' AND BiasDirection = 1 AND Completable = 1) AS TOWARDEND,
(SELECT COUNT(*) FROM Results WHERE Algorithm = 'Random' AND World = 'World5' AND Completable = 1) AS TOTAL)) AS R5Bd,

--Forward, World1
(SELECT CAST(TOWARDEND as float) / TOTAL FROM( SELECT
(SELECT COUNT(*) FROM Results WHERE Algorithm = 'Forward' AND World = 'World1' AND BiasDirection = 1 AND Completable = 1) AS TOWARDEND,
(SELECT COUNT(*) FROM Results WHERE Algorithm = 'Forward' AND World = 'World1' AND Completable = 1) AS TOTAL)) AS F1Bd,
--Forward, World2
(SELECT CAST(TOWARDEND as float) / TOTAL FROM( SELECT
(SELECT COUNT(*) FROM Results WHERE Algorithm = 'Forward' AND World = 'World2' AND BiasDirection = 1 AND Completable = 1) AS TOWARDEND,
(SELECT COUNT(*) FROM Results WHERE Algorithm = 'Forward' AND World = 'World2' AND Completable = 1) AS TOTAL)) AS F2Bd,
--Forward, World3
(SELECT CAST(TOWARDEND as float) / TOTAL FROM( SELECT
(SELECT COUNT(*) FROM Results WHERE Algorithm = 'Forward' AND World = 'World3' AND BiasDirection = 1 AND Completable = 1) AS TOWARDEND,
(SELECT COUNT(*) FROM Results WHERE Algorithm = 'Forward' AND World = 'World3' AND Completable = 1) AS TOTAL)) AS F3Bd,
--Forward, World4
(SELECT CAST(TOWARDEND as float) / TOTAL FROM( SELECT
(SELECT COUNT(*) FROM Results WHERE Algorithm = 'Forward' AND World = 'World4' AND BiasDirection = 1 AND Completable = 1) AS TOWARDEND,
(SELECT COUNT(*) FROM Results WHERE Algorithm = 'Forward' AND World = 'World4' AND Completable = 1) AS TOTAL)) AS F4Bd,
--Forward, World5
(SELECT CAST(TOWARDEND as float) / TOTAL FROM( SELECT
(SELECT COUNT(*) FROM Results WHERE Algorithm = 'Forward' AND World = 'World5' AND BiasDirection = 1 AND Completable = 1) AS TOWARDEND,
(SELECT COUNT(*) FROM Results WHERE Algorithm = 'Forward' AND World = 'World5' AND Completable = 1) AS TOTAL)) AS F5Bd,

--Assumed, World1
(SELECT CAST(TOWARDEND as float) / TOTAL FROM( SELECT
(SELECT COUNT(*) FROM Results WHERE Algorithm = 'Assumed' AND World = 'World1' AND BiasDirection = 1 AND Completable = 1) AS TOWARDEND,
(SELECT COUNT(*) FROM Results WHERE Algorithm = 'Assumed' AND World = 'World1' AND Completable = 1) AS TOTAL)) AS A1Bd,
--Assumed, World2
(SELECT CAST(TOWARDEND as float) / TOTAL FROM( SELECT
(SELECT COUNT(*) FROM Results WHERE Algorithm = 'Assumed' AND World = 'World2' AND BiasDirection = 1 AND Completable = 1) AS TOWARDEND,
(SELECT COUNT(*) FROM Results WHERE Algorithm = 'Assumed' AND World = 'World2' AND Completable = 1) AS TOTAL)) AS A2Bd,
--Assumed, World3
(SELECT CAST(TOWARDEND as float) / TOTAL FROM( SELECT
(SELECT COUNT(*) FROM Results WHERE Algorithm = 'Assumed' AND World = 'World3' AND BiasDirection = 1 AND Completable = 1) AS TOWARDEND,
(SELECT COUNT(*) FROM Results WHERE Algorithm = 'Assumed' AND World = 'World3' AND Completable = 1) AS TOTAL)) AS A3Bd,
--Assumed, World4
(SELECT CAST(TOWARDEND as float) / TOTAL FROM( SELECT
(SELECT COUNT(*) FROM Results WHERE Algorithm = 'Assumed' AND World = 'World4' AND BiasDirection = 1 AND Completable = 1) AS TOWARDEND,
(SELECT COUNT(*) FROM Results WHERE Algorithm = 'Assumed' AND World = 'World4' AND Completable = 1) AS TOTAL)) AS A4Bd,
--Assumed, World5
(SELECT CAST(TOWARDEND as float) / TOTAL FROM( SELECT
(SELECT COUNT(*) FROM Results WHERE Algorithm = 'Assumed' AND World = 'World5' AND BiasDirection = 1 AND Completable = 1) AS TOWARDEND,
(SELECT COUNT(*) FROM Results WHERE Algorithm = 'Assumed' AND World = 'World5' AND Completable = 1) AS TOTAL)) AS A5Bd,

-----Average Interestingness Per Algo Per World-----
--Random, World1
(SELECT AVG(Interestingness) FROM Results WHERE Algorithm = 'Random' AND World = 'World1' AND Completable = 1) AS R1I,
--Random, World2
(SELECT AVG(Interestingness) FROM Results WHERE Algorithm = 'Random' AND World = 'World2' AND Completable = 1) AS R2I,
--Random, World3
(SELECT AVG(Interestingness) FROM Results WHERE Algorithm = 'Random' AND World = 'World3' AND Completable = 1) AS R3I,
--Random, World4
(SELECT AVG(Interestingness) FROM Results WHERE Algorithm = 'Random' AND World = 'World4' AND Completable = 1) AS R4I,
--Random, World5
(SELECT AVG(Interestingness) FROM Results WHERE Algorithm = 'Random' AND World = 'World5' AND Completable = 1) AS R5I,

--Forward, World1
(SELECT AVG(Interestingness) FROM Results WHERE Algorithm = 'Forward' AND World = 'World1' AND Completable = 1) AS F1I,
--Forward, World2
(SELECT AVG(Interestingness) FROM Results WHERE Algorithm = 'Forward' AND World = 'World2' AND Completable = 1) AS F2I,
--Forward, World3
(SELECT AVG(Interestingness) FROM Results WHERE Algorithm = 'Forward' AND World = 'World3' AND Completable = 1) AS F3I,
--Forward, World4
(SELECT AVG(Interestingness) FROM Results WHERE Algorithm = 'Forward' AND World = 'World4' AND Completable = 1) AS F4I,
--Forward, World5
(SELECT AVG(Interestingness) FROM Results WHERE Algorithm = 'Forward' AND World = 'World5' AND Completable = 1) AS F5I,

--Assumed, World1
(SELECT AVG(Interestingness) FROM Results WHERE Algorithm = 'Assumed' AND World = 'World1' AND Completable = 1) AS A1I,
--Assumed, World2
(SELECT AVG(Interestingness) FROM Results WHERE Algorithm = 'Assumed' AND World = 'World2' AND Completable = 1) AS A2I,
--Assumed, World3
(SELECT AVG(Interestingness) FROM Results WHERE Algorithm = 'Assumed' AND World = 'World3' AND Completable = 1) AS A3I,
--Assumed, World4
(SELECT AVG(Interestingness) FROM Results WHERE Algorithm = 'Assumed' AND World = 'World4' AND Completable = 1) AS A4I,
--Assumed, World5
(SELECT AVG(Interestingness) FROM Results WHERE Algorithm = 'Assumed' AND World = 'World5' AND Completable = 1) AS A5I,

-----Average Fun Per Algo Per World-----
--Random, World1
(SELECT AVG(Fun) FROM Results WHERE Algorithm = 'Random' AND World = 'World1' AND Completable = 1) AS R1F,
--Random, World2
(SELECT AVG(Fun) FROM Results WHERE Algorithm = 'Random' AND World = 'World2' AND Completable = 1) AS R2F,
--Random, World3
(SELECT AVG(Fun) FROM Results WHERE Algorithm = 'Random' AND World = 'World3' AND Completable = 1) AS R3F,
--Random, World4
(SELECT AVG(Fun) FROM Results WHERE Algorithm = 'Random' AND World = 'World4' AND Completable = 1) AS R4F,
--Random, World5
(SELECT AVG(Fun) FROM Results WHERE Algorithm = 'Random' AND World = 'World5' AND Completable = 1) AS R5F,

--Forward, World1
(SELECT AVG(Fun) FROM Results WHERE Algorithm = 'Forward' AND World = 'World1' AND Completable = 1) AS F1F,
--Forward, World2
(SELECT AVG(Fun) FROM Results WHERE Algorithm = 'Forward' AND World = 'World2' AND Completable = 1) AS F2F,
--Forward, World3
(SELECT AVG(Fun) FROM Results WHERE Algorithm = 'Forward' AND World = 'World3' AND Completable = 1) AS F3F,
--Forward, World4
(SELECT AVG(Fun) FROM Results WHERE Algorithm = 'Forward' AND World = 'World4' AND Completable = 1) AS F4F,
--Forward, World5
(SELECT AVG(Fun) FROM Results WHERE Algorithm = 'Forward' AND World = 'World5' AND Completable = 1) AS F5F,

--Assumed, World1
(SELECT AVG(Fun) FROM Results WHERE Algorithm = 'Assumed' AND World = 'World1' AND Completable = 1) AS A1F,
--Assumed, World2
(SELECT AVG(Fun) FROM Results WHERE Algorithm = 'Assumed' AND World = 'World2' AND Completable = 1) AS A2F,
--Assumed, World3
(SELECT AVG(Fun) FROM Results WHERE Algorithm = 'Assumed' AND World = 'World3' AND Completable = 1) AS A3F,
--Assumed, World4
(SELECT AVG(Fun) FROM Results WHERE Algorithm = 'Assumed' AND World = 'World4' AND Completable = 1) AS A4F,
--Assumed, World5
(SELECT AVG(Fun) FROM Results WHERE Algorithm = 'Assumed' AND World = 'World5' AND Completable = 1) AS A5F,

-----Average Challenge Per Algo Per World-----
--Random, World1
(SELECT AVG(Challenge) FROM Results WHERE Algorithm = 'Random' AND World = 'World1' AND Completable = 1) AS R1C,
--Random, World2
(SELECT AVG(Challenge) FROM Results WHERE Algorithm = 'Random' AND World = 'World2' AND Completable = 1) AS R2C,
--Random, World3
(SELECT AVG(Challenge) FROM Results WHERE Algorithm = 'Random' AND World = 'World3' AND Completable = 1) AS R3C,
--Random, World4
(SELECT AVG(Challenge) FROM Results WHERE Algorithm = 'Random' AND World = 'World4' AND Completable = 1) AS R4C,
--Random, World5
(SELECT AVG(Challenge) FROM Results WHERE Algorithm = 'Random' AND World = 'World5' AND Completable = 1) AS R5C,

--Forward, World1
(SELECT AVG(Challenge) FROM Results WHERE Algorithm = 'Forward' AND World = 'World1' AND Completable = 1) AS F1C,
--Forward, World2
(SELECT AVG(Challenge) FROM Results WHERE Algorithm = 'Forward' AND World = 'World2' AND Completable = 1) AS F2C,
--Forward, World3
(SELECT AVG(Challenge) FROM Results WHERE Algorithm = 'Forward' AND World = 'World3' AND Completable = 1) AS F3C,
--Forward, World4
(SELECT AVG(Challenge) FROM Results WHERE Algorithm = 'Forward' AND World = 'World4' AND Completable = 1) AS F4C,
--Forward, World5
(SELECT AVG(Challenge) FROM Results WHERE Algorithm = 'Forward' AND World = 'World5' AND Completable = 1) AS F5C,

--Assumed, World1
(SELECT AVG(Challenge) FROM Results WHERE Algorithm = 'Assumed' AND World = 'World1' AND Completable = 1) AS A1C,
--Assumed, World2
(SELECT AVG(Challenge) FROM Results WHERE Algorithm = 'Assumed' AND World = 'World2' AND Completable = 1) AS A2C,
--Assumed, World3
(SELECT AVG(Challenge) FROM Results WHERE Algorithm = 'Assumed' AND World = 'World3' AND Completable = 1) AS A3C,
--Assumed, World4
(SELECT AVG(Challenge) FROM Results WHERE Algorithm = 'Assumed' AND World = 'World4' AND Completable = 1) AS A4C,
--Assumed, World5
(SELECT AVG(Challenge) FROM Results WHERE Algorithm = 'Assumed' AND World = 'World5' AND Completable = 1) AS A5C,

-----Average Satisfyingness Per Algo Per World-----
--Random, World1
(SELECT AVG(Satisfyingness) FROM Results WHERE Algorithm = 'Random' AND World = 'World1' AND Completable = 1) AS R1S,
--Random, World2
(SELECT AVG(Satisfyingness) FROM Results WHERE Algorithm = 'Random' AND World = 'World2' AND Completable = 1) AS R2S,
--Random, World3
(SELECT AVG(Satisfyingness) FROM Results WHERE Algorithm = 'Random' AND World = 'World3' AND Completable = 1) AS R3S,
--Random, World4
(SELECT AVG(Satisfyingness) FROM Results WHERE Algorithm = 'Random' AND World = 'World4' AND Completable = 1) AS R4S,
--Random, World5
(SELECT AVG(Satisfyingness) FROM Results WHERE Algorithm = 'Random' AND World = 'World5' AND Completable = 1) AS R5S,

--Forward, World1
(SELECT AVG(Satisfyingness) FROM Results WHERE Algorithm = 'Forward' AND World = 'World1' AND Completable = 1) AS F1S,
--Forward, World2
(SELECT AVG(Satisfyingness) FROM Results WHERE Algorithm = 'Forward' AND World = 'World2' AND Completable = 1) AS F2S,
--Forward, World3
(SELECT AVG(Satisfyingness) FROM Results WHERE Algorithm = 'Forward' AND World = 'World3' AND Completable = 1) AS F3S,
--Forward, World4
(SELECT AVG(Satisfyingness) FROM Results WHERE Algorithm = 'Forward' AND World = 'World4' AND Completable = 1) AS F4S,
--Forward, World5
(SELECT AVG(Satisfyingness) FROM Results WHERE Algorithm = 'Forward' AND World = 'World5' AND Completable = 1) AS F5S,

--Assumed, World1
(SELECT AVG(Satisfyingness) FROM Results WHERE Algorithm = 'Assumed' AND World = 'World1' AND Completable = 1) AS A1S,
--Assumed, World2
(SELECT AVG(Satisfyingness) FROM Results WHERE Algorithm = 'Assumed' AND World = 'World2' AND Completable = 1) AS A2S,
--Assumed, World3
(SELECT AVG(Satisfyingness) FROM Results WHERE Algorithm = 'Assumed' AND World = 'World3' AND Completable = 1) AS A3S,
--Assumed, World4
(SELECT AVG(Satisfyingness) FROM Results WHERE Algorithm = 'Assumed' AND World = 'World4' AND Completable = 1) AS A4S,
--Assumed, World5
(SELECT AVG(Satisfyingness) FROM Results WHERE Algorithm = 'Assumed' AND World = 'World5' AND Completable = 1) AS A5S,

-----Average Boredom Per Algo Per World-----
--Random, World1
(SELECT AVG(Boredom) FROM Results WHERE Algorithm = 'Random' AND World = 'World1' AND Completable = 1) AS R1Bo,
--Random, World2
(SELECT AVG(Boredom) FROM Results WHERE Algorithm = 'Random' AND World = 'World2' AND Completable = 1) AS R2Bo,
--Random, World3
(SELECT AVG(Boredom) FROM Results WHERE Algorithm = 'Random' AND World = 'World3' AND Completable = 1) AS R3Bo,
--Random, World4
(SELECT AVG(Boredom) FROM Results WHERE Algorithm = 'Random' AND World = 'World4' AND Completable = 1) AS R4Bo,
--Random, World5
(SELECT AVG(Boredom) FROM Results WHERE Algorithm = 'Random' AND World = 'World5' AND Completable = 1) AS R5Bo,

--Forward, World1
(SELECT AVG(Boredom) FROM Results WHERE Algorithm = 'Forward' AND World = 'World1' AND Completable = 1) AS F1Bo,
--Forward, World2
(SELECT AVG(Boredom) FROM Results WHERE Algorithm = 'Forward' AND World = 'World2' AND Completable = 1) AS F2Bo,
--Forward, World3
(SELECT AVG(Boredom) FROM Results WHERE Algorithm = 'Forward' AND World = 'World3' AND Completable = 1) AS F3Bo,
--Forward, World4
(SELECT AVG(Boredom) FROM Results WHERE Algorithm = 'Forward' AND World = 'World4' AND Completable = 1) AS F4Bo,
--Forward, World5
(SELECT AVG(Boredom) FROM Results WHERE Algorithm = 'Forward' AND World = 'World5' AND Completable = 1) AS F5Bo,

--Assumed, World1
(SELECT AVG(Boredom) FROM Results WHERE Algorithm = 'Assumed' AND World = 'World1' AND Completable = 1) AS A1Bo,
--Assumed, World2
(SELECT AVG(Boredom) FROM Results WHERE Algorithm = 'Assumed' AND World = 'World2' AND Completable = 1) AS A2Bo,
--Assumed, World3
(SELECT AVG(Boredom) FROM Results WHERE Algorithm = 'Assumed' AND World = 'World3' AND Completable = 1) AS A3Bo,
--Assumed, World4
(SELECT AVG(Boredom) FROM Results WHERE Algorithm = 'Assumed' AND World = 'World4' AND Completable = 1) AS A4Bo,
--Assumed, World5
(SELECT AVG(Boredom) FROM Results WHERE Algorithm = 'Assumed' AND World = 'World5' AND Completable = 1) AS A5Bo