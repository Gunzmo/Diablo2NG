-- --------------------------------------------------------
-- Värd:                         localhost
-- Server version:               5.1.73-community - MySQL Community Server (GPL)
-- Server OS:                    Win64
-- HeidiSQL Version:             9.2.0.4947
-- --------------------------------------------------------

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;

-- Dumping database structure for d2ng
CREATE DATABASE IF NOT EXISTS `d2ng` /*!40100 DEFAULT CHARACTER SET utf8 */;
USE `d2ng`;


-- Dumping structure for table d2ng.games
CREATE TABLE IF NOT EXISTS `games` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `Game` varchar(50) NOT NULL,
  `Password` varchar(50) NOT NULL,
  `Description` text NOT NULL,
  `Difficulty` varchar(50) NOT NULL,
  `Realm` varchar(50) NOT NULL,
  `ladder` int(11) NOT NULL,
  `BotGame` int(11) NOT NULL DEFAULT '0',
  `Empty` int(1) NOT NULL DEFAULT '0',
  `created` int(20) NOT NULL,
  `timestamp` int(20) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=69 DEFAULT CHARSET=utf8;

-- Dumping data for table d2ng.games: ~2 rows (approximately)
/*!40000 ALTER TABLE `games` DISABLE KEYS */;
INSERT INTO `games` (`id`, `Game`, `Password`, `Description`, `Difficulty`, `Realm`, `ladder`, `BotGame`, `Empty`, `created`, `timestamp`) VALUES
	(1, 'fake', '1', 'Huehue', 'Hell', '3', 1, 0, 0, 0, 0),
	(68, 'fakeGame', '10', 'WooT test description', 'Normal', '3', 0, 0, 0, 1444409167, 1444409294);
/*!40000 ALTER TABLE `games` ENABLE KEYS */;


-- Dumping structure for table d2ng.hwid
CREATE TABLE IF NOT EXISTS `hwid` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `IP` text NOT NULL,
  `HWID` char(64) DEFAULT NULL,
  `gm` int(1) NOT NULL DEFAULT '0',
  `banned` int(11) NOT NULL DEFAULT '0',
  `reason` text,
  `timestamp` int(20) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `HWID` (`HWID`),
  KEY `ID` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=44 DEFAULT CHARSET=utf8;

-- Dumping data for table d2ng.hwid: ~1 rows (approximately)
/*!40000 ALTER TABLE `hwid` DISABLE KEYS */;
/*!40000 ALTER TABLE `hwid` ENABLE KEYS */;


-- Dumping structure for table d2ng.players
CREATE TABLE IF NOT EXISTS `players` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `GameID` int(11) NOT NULL DEFAULT '0',
  `Name` varchar(255) DEFAULT NULL,
  `timestamp` int(20) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=10 DEFAULT CHARSET=utf8;

-- Dumping data for table d2ng.players: ~7 rows (approximately)
/*!40000 ALTER TABLE `players` DISABLE KEYS */;
INSERT INTO `players` (`id`, `GameID`, `Name`, `timestamp`) VALUES
	(3, 68, 'Fake 1 Sorceress level 0', NULL),
	(4, 68, '2  Sorceress level 0', NULL),
	(5, 68, '3  Sorceress level 0', NULL),
	(6, 68, '4  Sorceress level 0', NULL),
	(7, 68, '5  Sorceress level 0', NULL),
	(8, 68, '6  Sorceress level 0', NULL),
	(9, 68, '7  Sorceress level 0', NULL);
/*!40000 ALTER TABLE `players` ENABLE KEYS */;


-- Dumping structure for table d2ng.user
CREATE TABLE IF NOT EXISTS `user` (
  `id` int(11) NOT NULL,
  `playerID` int(11) NOT NULL,
  `HWID` int(11) NOT NULL,
  `user` varchar(50) DEFAULT NULL,
  `hash` varchar(50) DEFAULT NULL,
  `realm` int(11) NOT NULL DEFAULT '0',
  `timestamp` int(11) NOT NULL DEFAULT '0',
  UNIQUE KEY `playerID` (`playerID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- Dumping data for table d2ng.user: ~1 rows (approximately)
/*!40000 ALTER TABLE `user` DISABLE KEYS */;
/*!40000 ALTER TABLE `user` ENABLE KEYS */;
/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IF(@OLD_FOREIGN_KEY_CHECKS IS NULL, 1, @OLD_FOREIGN_KEY_CHECKS) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
