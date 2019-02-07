<?php
//NetworkLogin Discord Bot Requests Processor
//Copyright by Łukasz Jurczyk <zabszk [at] protonmail [dot] ch>, 2019

$minecraftservertoken = ""; //set LONG random token here (different than the other one in second php file), KEEP SECRET LIKE A PASSWORD!

$dbhost = "localhost";
$dbport = 3306;
$dbname = "NetworkLogin";
$dbtablename = "NetworkLogin";
$dbuser = "";
$dbpassword = "";

if (empty($_POST['username']) || empty($_POST['token'])) die('Missing data');
if ($_POST['token'] != $minecraftservertoken) die('Invalid token');
$pdo = new PDO('mysql:host=' . $dbhost . ';dbname=' . $dbname . ';port=' . $dbport . ';charset=utf8', $dbuser, $dbpassword, array(PDO::MYSQL_ATTR_INIT_COMMAND => "SET NAMES utf8"));
$pdo->setAttribute(PDO::ATTR_EMULATE_PREPARES, false);
$pdo->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);
$stmt = $pdo->prepare("SELECT `Password` FROM `" . $dbtablename . "` WHERE `Username` = :login LIMIT 1");
$stmt->bindValue(':login', $_POST['username'], PDO::PARAM_STR);
$stmt->execute();
$row = $stmt->fetch();
if ($row == null) die ('User not found');
if (empty($_POST['password'])) die('Permitted to join');
if (password_verify($_POST['password'], $row['Password'])) echo 'Authenticated';
else echo 'Rejected';
