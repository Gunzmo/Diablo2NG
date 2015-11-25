<?php
	//Define and Decrypy
		error_reporting(E_ALL);
		require_once "conf.php";
		header("Content-Type:text/plain");
		$key = "wGNuqvkYNrRxKNfGN2cS50oVfTIW1FRAlW0otF3KLxo=";
		$iv = "bT31M32Ik5TKkCS/dvasSTJHwcHdKyCZ1ix7LHwVWIA=";
		$date = date_timestamp_get(date_create());
		$msg = decrypt($key,$iv,$HTTP_RAW_POST_DATA);
		$msgs = explode("|", $msg);
		function strippadding($string)
			{
			    $slast = ord(substr($string, -1));
			    $slastc = chr($slast);
			    $pcheck = substr($string, -$slast);
			    if(preg_match("/$slastc{".$slast."}/", $string)){
			        $string = substr($string, 0, strlen($string)-$slast);
			        return $string;
			    } else {
			        return false;
			    }
			}
		function addpadding($string, $blocksize = 32)
		{
		    $len = strlen($string);
		    $pad = $blocksize - ($len % $blocksize);
		    $string .= str_repeat(chr($pad), $pad);
		    return $string;
		}

		function decrypt($key, $iv, $string = "")
		{
			$key = base64_decode($key);
			$iv = base64_decode($iv);
			$string = base64_decode($string);
			return strippadding(mcrypt_decrypt(MCRYPT_RIJNDAEL_256, $key, $string, MCRYPT_MODE_CBC, $iv));
		}
		function encrypt($string = "")
		{
		    $key = base64_decode("wGNuqvkYNrRxKNfGN2cS50oVfTIW1FRAlW0otF3KLxo=");
		    $iv = base64_decode("bT31M32Ik5TKkCS/dvasSTJHwcHdKyCZ1ix7LHwVWIA=");
		    return base64_encode(mcrypt_encrypt(MCRYPT_RIJNDAEL_256, $key, addpadding($string), MCRYPT_MODE_CBC, $iv));
		}


//PHP D2NG Server
		switch ($msgs[0]) {
			case '1': //Login
				Login($msgs[1],$msgs[2],$msgs[3],$date,0);
				break;
			case '2': //CreateGame
				CreateGame($msgs[1],$msgs[2],$msgs[3],$msgs[4],$msgs[5],$msgs[6],$msgs[7],$msgs[8],$date);
				break;
			case '3':// Request to remove
				RequestToRemove($msgs[1], $date);
				break;
			case '4': //Request GameList and data
				requstGameList($msgs[1], $date, $msgs[2], $msgs[3]);
				break;
			case '5': //JoinGame
				JoinGame($msgs[1], $msgs[2], $msgs[3], $date);
				break;
			case '6': //ping!
				Ping($msgs[1], $date);
			break;
			default:
				foreach ($msgs as $key) 
				{
					echo "Erorr: " + $key;
				}
				break;
		}
		function JoinGame($Id, $Char, $Hash, $timestamp)
		{
			$once = false;
			include "conf.php";
			$conn = new mysqli($serverip, $username, $password, $dbname, $Port);
			if ($conn->connect_error) 
			{
			    die("Connection failed: " . $conn->connect_error);
			} 
			$id = mysqli_escape_string($conn, $Id);
			$char = mysqli_escape_string($conn, $Char);
			$hash = mysqli_escape_string($conn, $Hash);
			$UserQuerry = "SELECT * from user where hash = '$hash'";
			if ($UserResult->num_rows > 0) 
			{
			    while($UserRow = $UserResult->fetch_assoc()) 
			    {
			    	if($once == false)
			    	{
			    		$once = true;
				    	$BanCheckQuerrt = "SELECT * from hwid where id = '".$UserRow['id']."'";
				    	$BanCheckResult = $conn->query($BanCheckQuerrt);
				    	while($BanCheckRow = $BanCheckResult->fetch_assoc()) 
				    	{
				    		if((int)$BanCheckRow['banned'] == 0)
				    		{
				    			$once = true;
				    			$sqli = "INSERT INTO Players (GameID, Name, timestamp) Values ('$id', '$char', '$timestamp')";
				    			$updateUserQuerry = "UPDATE user SET playerID = '" .mysqli_insert_id($conn). "' where hash = '$hash'";
				    			$updateUserTimeStampQuerry = "UPDATE user SET timestamp = '$timestamp' where hash = '$hash'";
							    $conn->query($updateUserQuerry);
							    $conn->query($updateUserTimeStampQuerry);
				    			$conn->query($sqli);
				    		}
				    		else
				    			echo encrypt("Banned");
				    	}
				    }
			    }
			}
		}
		function requstGameList($Hash, $timestamp, $Lad, $Type = 0)
		{
			$once = false;
			$uno = false;
			$up = 0;
			include "conf.php";
			$conn = new mysqli($serverip, $username, $password, $dbname, $Port);
			if ($conn->connect_error) {
			    die("Connection failed: " . $conn->connect_error);
			} 
			$hash = mysqli_escape_string($conn, $Hash);
			$lad = mysqli_escape_string($conn, $Lad);
			$type = mysqli_escape_string($conn, $Type);
			$UserQuerry = "SELECT * from user where hash = '$hash'";
			$UserResult = $conn->query($UserQuerry);
			if ($UserResult->num_rows > 0) 
			{
			    while($UserRow = $UserResult->fetch_assoc()) 
			    {
			    	if($once == false)
			    	{
				    	$BanCheckQuerrt = "SELECT * from hwid where id = '".$UserRow['id']."'";
				    	$BanCheckResult = $conn->query($BanCheckQuerrt);
				    	while($BanCheckRow = $BanCheckResult->fetch_assoc()) 
				    	{
				    		if((int)$BanCheckRow['banned'] == 0)
				    		{
				    			//Ping($hash, $timestamp, 0);
								$gameResult = $conn->query("SELECT * from games where BotGame = '$type' AND ladder = '$lad'");
								if ($gameResult->num_rows > 0) 
								{
								    while($GameRow = $gameResult->fetch_assoc()) 
								    {
								    	$PlayerResult = $conn->query("SELECT * from players where GameID = '".$GameRow['id']."'");
								    	if ($PlayerResult->num_rows < 8) 
										{
										    $up = ((int)$GameRow['created'] - $timestamp);
						    				echo encrypt($GameRow['id'] . "■" . $GameRow['Game'] . "■" . $GameRow['Password'] . "■" . $GameRow['Description'] . "■" . $GameRow['Difficulty'] . "■" . $GameRow['Realm']. "■" . $up . "■" . getPlayers($GameRow['id'],$conn, 2) . "|");
										}
									}
				    			}
				    			else
				    				printf(encrypt(",,,,,,,,"));
					    	}
					    	else
			    				printf(encrypt("Banned"));
			    		}
		    		}
				}
			}
			else
		{
			echo encrypt("No hash");
			return;
		}
			$conn->close();
	    }
	    function getPlayers($GameID, $conn,$mode = 0)
	    {
	    	$holder = "";
	    	$PlayerQuerry = "SELECT * from players where GameID = '$GameID'";
			$PlayerResult = $conn->query($PlayerQuerry);
			if ($PlayerResult->num_rows > 0) 
			{
			    while($row = $PlayerResult->fetch_assoc())
			    {
				    {
				    	switch ($mode) 
				    	{
				    		case 0:
			    				$holder .= $row["id"]. ".";
				    			break;
				    		case 1:
				    			$holder .= $row["GameID"]. ".";
				    			break;
				    		case 2:
				    			$holder .= $row["Name"]. ".";
				    			break;
				    		case 3:
				    			$holder .= $row["timestamp"]. ".";
				    			break;
			    			case 4:
				    			$holder .= $row["id"]. ".";
				    			break;
				    	}
		    		}
	    		}
			}
	    	return $holder;
		}
		function Ping($Hash, $timestamp, $ingame = 1)
		{
			include "conf.php";
			$once = false;
			$uno = false;
			$conn = new mysqli($serverip, $username, $password, $dbname, $Port);
			if ($conn->connect_error) {
			    die("Connection failed: " . $conn->connect_error);
			} 
			$hash = mysqli_escape_string($conn, $Hash);
			$UserQuerry = "SELECT * from user where hash = '$hash'";
			$UserResult = $conn->query($UserQuerry);
			if ($UserResult->num_rows > 0) 
			{
			    while($UserRow = $UserResult->fetch_assoc()) 
			    {
			    	if(!$once)
			    	{
			    		$once = true;
			    		if($ingame == 0)
			    		{
				    		if($timestamp - 30 <= $UserRow["timestamp"])
				    		{
				    			$BanQuerry = "UPDATE user SET banned ='1' where id = '".$UserRow["id"]."'";
				    			$conn->query($BanQuerry);
				    		}
			    		}
			    		
				    	if($ingame == 1)
				    	{
				    		$PlayerQuerry = "UPDATE player SET timestamp ='$timestamp' where id = '".$UserRow["playerID"]."'"; //Character
				    		$conn->query($PlayerQuerry);
				    		$SelectPlayerQuerry = "SELECT * from players where id = '".$UserRow["playerID"]."'";
							$SelectPlayerResult = $conn->query($SelectPlayerQuerry);
							if ($SelectPlayerResult->num_rows > 0) 
							{
							    while($PlayerRow = $SelectPlayerResult->fetch_assoc()) 
							    {
							    	if ($uno == false)
							    	{
							    		UpdateGame($PlayerRow["GameID"], $timestamp); //In Game Update!
							    		$uno = true;
							    	}
							    	
							    }
							}
				    	}
						$UserQuerry = "UPDATE user SET timestamp ='$timestamp' where hash = '$hash'"; // Account
						$conn->query($UserQuerry);
						echo encrypt("Pong");
					}
			    }
			}
			else
			{
				echo encrypt("No Hash");
				return;
			}
		}
		function UpdateGame($GameID, $timestamp)
		{
			include "conf.php";
			$conn = new mysqli($serverip, $username, $password, $dbname, $Port);
			if ($conn->connect_error) {
			    die("Connection failed: " . $conn->connect_error);
			} 
			$gamesUpdateQuerry = "UPDATE games SET timestamp ='$timestamp' where id = '$GameID'";
				$gamesUpdateResult = $conn->query($gamesUpdateQuerry);
				if (!$gamesUpdateResult) 
				{
				    echo encrypt("Can't UPDATE game! ID: $GameID Error: ". $conn->error);
				}

				$PlayerQuerry = "SELECT * from players where GameID = '$GameID'";
	   			$PlayerResult = $conn->query($PlayerQuerry);
	   			//printf("Playes:" .$PlayerResult->num_rows);
			    if ($PlayerResult->num_rows == 0) 
			    {
			    	$gameUpdateEmptyQuerry = "UPDATE games SET Empty=1 where id = '$GameID'";
			    	$conn->query($gameUpdateEmptyQuerry);
			    }
			    else if ($PlayerResult->num_rows >= 1) 
			    {
			    	$gameUpdateEmptyQuerry = "UPDATE games SET Empty=0 where id = '$GameID'";
			    	$conn->query($gameUpdateEmptyQuerry);
			    }

			$gameQuerry = "SELECT * from games where id = '$GameID'";
       		$gameResult = $conn->query($gameQuerry);
       		if ($gameResult->num_rows > 0) 
       		{
       			while($GameRow = $gameResult->fetch_assoc()) 
	       		{
				    if((int)$GameRow['timestamp'] <= ($timestamp - 30) && (int)$GameRow['Empty'] == 1)
				    {
				    	$gameDeleteQuerry = "DELETE FROM games where id = '$GameID'";
				    	$conn->query($gameDeleteQuerry);
				    	print encrypt("GameDeleted:$gameID");
				    }
				}
			} 
			else {echo encrypt("0 results");}
			$conn->close();
		}
		function RequestToRemove($Hash, $timestamp)
		{
			include "conf.php";
			$doOnce = false;
			$conn = new mysqli($serverip, $username, $password, $dbname, $Port);
			if ($conn->connect_error) {
			    die("Connection failed: " . $conn->connect_error);
			} 
			$hash = mysqli_escape_string($conn, $Hash);
			$UserQuerry = "SELECT * from User where hash = '$hash'";
			$UserResult = $conn->query($UserQuerry);
			if (!$UserResult) {
			     echo encrypt("hash not found: ".$hash);
			}
			if ($UserResult->num_rows > 0) 
			{
			    while($UserRow = $UserResult->fetch_assoc()) 
			    {
			        $PlayerQuerry = "SELECT * from Players where ID = '".$UserRow['playerID']."'";
		    	    $PlayerResult = $conn->query($PlayerQuerry);
			        while($PlayerRow = $PlayerResult->fetch_assoc()) 
			        {
			       		$gameID = $PlayerRow['GameID'];
			       		if(!$doOnce)
			       		{
			       			$doOnce = true;
						    $playerDeleteQuerry = "DELETE FROM Players where id = '".$UserRow['playerID']."'";
							$playerDeleteResult = $conn->query($playerDeleteQuerry);
							UpdateGame($gameID, $timestamp);
						}
			        }
			    }
			}
			else
			{
				echo encrypt("No Hash");
			}
			$conn->close();
			return;
		}
		function CreateGame($Game, $Pass, $Diff, $CharInfo, $Lad, $Desc, $Realm, $Hash, $timestamp, $Bot = 0)
		{
			include "conf.php";
			$conn = new mysqli($serverip, $username, $password, $dbname, $Port);
			if ($conn->connect_error) {
			    die("Connection failed: " . $conn->connect_error);
			} 
			$game = mysqli_escape_string($conn, $Game);
			$pass = mysqli_escape_string($conn, $Pass);
			$diff = mysqli_escape_string($conn, $Diff);
			$hash = mysqli_escape_string($conn, $Hash);
			$charInfo = mysqli_escape_string($conn, $CharInfo);
			$desc = mysqli_escape_string($conn, $Desc);
			$realm = substr($Realm, 0, 1);
			$realm = mysqli_escape_string($conn, $realm);
			$bot = mysqli_escape_string($conn, $Bot);
			$lad = mysqli_escape_string($conn, $Lad);
			$UserResult = $conn->query("SELECT * from User where hash = '$hash'");
			if (!$UserResult) 
			{
			    echo encrypt("hash not found: ". $hash);
			}
			while($row = $UserResult->fetch_assoc()) 
		    {
		    	$BanCheckQuerrt = "SELECT * from hwid where id = '".$row['id']."'";
		    	$BanCheckResult = $conn->query($BanCheckQuerrt);
		    	while($BanCheckRow = $BanCheckResult->fetch_assoc()) 
		    	{
		    		if((int)$BanCheckRow['banned'] == 0)
		    		{
						$sql = "INSERT INTO games (Game, Password, Description, Difficulty, Realm, Ladder, BotGame, created, timestamp ) VALUES ('$game', '$pass', '$desc', '$diff', '$realm', 'lad', '$bot', '$timestamp', '$timestamp')";
						if ($conn->query($sql) === TRUE) {
							$sqli = "INSERT INTO Players (GameID, Name, timestamp) Values ('".mysqli_insert_id($conn)."', '$charInfo', '$timestamp')";
							if ($conn->query($sqli) === TRUE) {
								$updateUserQuerry = "UPDATE user SET playerID = '" .mysqli_insert_id($conn). "' where hash = '$hash'";
							    $conn->query($updateUserQuerry);
							    echo mysqli_insert_id($conn);
							} else {
							    echo "Error: " . $sqli . "\n" . $conn->error;
							}
						} else {
						    echo "Error: " . $sql . "\n" . $conn->error;
						}
					}
					else
						echo encrypt("Banned!");
				}
			}
			$conn->close();
		}
		
		function Login($HWID, $User, $Realm, $timestamp, $mode = 0)
		{
			include "conf.php";
			if(!empty($_SERVER['REMOTE_ADDR']))
				$ip = $_SERVER['REMOTE_ADDR'];
			else
				$ip = "";
			// Create connection
			$conn = new mysqli($serverip, $username, $password, $dbname, $Port);
			$hwid = mysqli_escape_string($conn, $HWID);
			$realm = substr($Realm, 0, 1);
			$realm = mysqli_escape_string($conn, $realm);
			$user = mysqli_escape_string($conn, $User);
			$sql;
			if ($conn->connect_error) {
			    die("Connection failed: " . $conn->connect_error);
			} 
			switch ($mode) {
				case '1':
					$sql = "SELECT * FROM HWID WHERE IP='".$ip."'";
					break;
				default:
					$sql = "SELECT * FROM HWID WHERE HWID='".$hwid."'";
					break;
			}
			$result = $conn->query($sql);
			$id;
			$hash = md5($HWID. rand(1,99999999));
			if ($result->num_rows > 0) {
			    while($row = $result->fetch_assoc()) 
			    {
			        if((int)$row["banned"] == 0)
			        {
			            if ($conn->query("UPDATE HWID Set ip = '$ip' WHERE hwid = '".$row["HWID"]."'") === TRUE)
			            {
			             	$conn->query("UPDATE user Set hash = '$hash' where HWID = '".$row["id"]."'");
			               	$conn->query("UPDATE user Set user = '$user' where HWID = '".$row["id"]."'");
			               	$conn->query("UPDATE user Set realm = '$realm' where HWID = '".$row["id"]."'");
			               	$conn->query("UPDATE HWID Set timestamp = '$timestamp' where HWID = '".$row["id"]."'");
			               	echo encrypt($hash);
			            } else {
			                echo encrypt("Error In Update, ;=0 HAxors Praxors Stop It Banned!");
			                $conn->query("UPDATE HWID Set banned = '1' WHERE hwid = '".$row["HWID"]."'");
			            }
			        }
			        else
			            echo encrypt("Banned, Reason: ".$row["reason"]);
			    }
			} else {
			      if ($conn->query("INSERT INTO HWID (hwid, ip, timestamp) VALUES ('$HWID', '$ip', '$timestamp')") === TRUE) {
			      		$id = mysqli_insert_id($conn);
			      		$sql = "INSERT INTO user (`HWID`, `user`, `hash`, `realm`, `timestamp`) Values ('$id', '$user', '$hash', '$realm', '$timestamp')";
			      		if ( $conn->query($sql) === TRUE) 
			          	{
			               	echo encrypt($hash);
			          	}
			          	else
			          	{
			          		echo encrypt("ID: $id\n");
			          		echo encrypt("Name: $user \n");
			          		echo encrypt("hash: $hash \n");
			          		echo encrypt("realm: $realm \n");
			          		echo encrypt("Error: " .$conn->error);
			          	}
			          	
			      } else {
			          echo encrypt("Banned!");
			      }
			}
			$conn->close();
		}
?>