CREATE SCHEMA IF NOT EXISTS `eb` DEFAULT CHARACTER SET utf8mb4 ;
use eb;
CREATE TABLE  IF NOT EXISTS `ebitems` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Skuid` varchar(45) DEFAULT NULL,
  `Name` varchar(500) CHARACTER SET utf8 DEFAULT NULL,
  `Price` decimal(10,2) DEFAULT NULL,
  `PriceDesc` text,
  `Gift` varchar(500) CHARACTER SET utf8 DEFAULT NULL,
  `PromotionDesc` text,
  `ProductUrl` varchar(100) CHARACTER SET utf8 DEFAULT NULL,
  `ElectricBusiness` varchar(20) CHARACTER SET utf8 DEFAULT NULL,
  `CreateTime` datetime DEFAULT NULL,
  `UpdateTime` datetime DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

