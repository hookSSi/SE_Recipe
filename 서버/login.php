<?php
   error_reporting(E_ALL);
   ini_set("display_errors", 1);
   require 'UserManager.php';

   $loginID = $_POST['loginID'];
   $loginPW = $_POST['loginPW'];

   $userManager = new UserManager();
   $result = $userManager->Login($loginID, $loginPW);

   echo json_encode($result, JSON_UNESCAPED_UNICODE | JSON_PRETTY_PRINT);
?>