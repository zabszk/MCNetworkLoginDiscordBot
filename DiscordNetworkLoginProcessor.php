<?php
//NetworkLogin Discord Bot Requests Processor
//Copyright by Łukasz Jurczyk <zabszk [at] protonmail [dot] ch>, 2019

$discordbotservertoken = ""; //set LONG random token here (different than the other one in second php file), KEEP SECRET LIKE A PASSWORD!

$dbhost = "localhost";
$dbport = 3306;
$dbname = "NetworkLogin";
$dbtablename = "NetworkLogin";
$dbuser = "";
$dbpassword = "";

$bcryptCost = 12;
$passwordMaxLen = 64;

if (empty($_POST['action']) || empty($_POST['token']) || empty($_POST['DiscordID'])) die('Missing data');
if ($_POST['token'] != $discordbotservertoken) die('Invalid token');

function GenerateRandom($l)
{
    return substr(str_replace("+", "", base64_encode(openssl_random_pseudo_bytes($l))), 0, $l);
}

$pdo = new PDO('mysql:host=' . $dbhost . ';dbname=' . $dbname . ';port=' . $dbport . ';charset=utf8', $dbuser, $dbpassword, array(PDO::MYSQL_ATTR_INIT_COMMAND => "SET NAMES utf8"));
$pdo->setAttribute(PDO::ATTR_EMULATE_PREPARES, false);
$pdo->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);

if ($_POST["action"] == "register") {
	if (empty($_POST['username'])) die('Missing data');
	$_POST['username'] = base64_decode($_POST['username']);
	$stmt = $pdo->prepare("SELECT `Password` FROM `" . $dbtablename . "` WHERE `Username` = :login LIMIT 1");
	$stmt->bindValue(':login', $_POST['username'], PDO::PARAM_STR);
	$stmt->execute();
	$row = $stmt->fetch();
	if ($row != null) die ('Username already registered');
	
	$stmt = $pdo->prepare("SELECT `Username` FROM `" . $dbtablename . "` WHERE `DiscordID` = :discord LIMIT 1");
	$stmt->bindValue(':discord', $_POST['DiscordID'], PDO::PARAM_STR);
	$stmt->execute();
	$row = $stmt->fetch();
	if ($row != null) die ('You already have account registered, username: `' . $row['Username'] . '`');
	
	$pass = GenerateRandom(24);
	
	$stmt = $pdo->prepare("INSERT INTO `" . $dbtablename . "` (`Username`, `DiscordID`, `Password`) VALUES (:username, :discord, :password)");
	$stmt->bindValue(':username', $_POST['username'], PDO::PARAM_STR);
	$stmt->bindValue(':discord', $_POST['DiscordID'], PDO::PARAM_STR);
	$stmt->bindValue(':password', $pass, PDO::PARAM_STR);
	$stmt->execute();
	
	echo "User created, password: `" . $pass . "` ";
}
else if ($_POST["action"] == "unregister") {
	$stmt = $pdo->prepare("SELECT `Username` FROM `" . $dbtablename . "` WHERE `DiscordID` = :discord LIMIT 1");
	$stmt->bindValue(':discord', $_POST['DiscordID'], PDO::PARAM_STR);
	$stmt->execute();
	$row = $stmt->fetch();
	if ($row == null) die ("You don't have any account registered.");
	
	$stmt = $pdo->prepare("DELETE FROM `" . $dbtablename . "` WHERE `DiscordID` = :discord");
	$stmt->bindValue(':discord', $_POST['DiscordID'], PDO::PARAM_STR);
	$stmt->execute();
	
	echo "Done";
}
else if ($_POST["action"] == "passwd") {
	if (empty($_POST['password'])) die('Missing data');
	$_POST['password'] = base64_decode($_POST['password']);
	if (strlen($_POST["password"]) > $passwordMaxLen) die ("Password cannot be longer than " . $passwordMaxLen . ".");
	
	$stmt = $pdo->prepare("SELECT `Username` FROM `" . $dbtablename . "` WHERE `DiscordID` = :discord LIMIT 1");
	$stmt->bindValue(':discord', $_POST['DiscordID'], PDO::PARAM_STR);
	$stmt->execute();
	$row = $stmt->fetch();
	if ($row == null) die ("You don't have any account registered.");
	
	$options = [
        'cost' => $bcryptCost
    ];
	
	$stmt = $pdo->prepare("UPDATE `" . $dbtablename . " SET `Password` = :password WHERE `DiscordID` = :discord)");
	$stmt->bindValue(':discord', $_POST['DiscordID'], PDO::PARAM_STR);
	$stmt->bindValue(':password', password_hash($_POST["password"], PASSWORD_BCRYPT, $options), PDO::PARAM_STR);
	$stmt->execute();
	
	echo "Done";
}
else echo "Unknown action";
