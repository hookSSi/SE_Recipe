<?php
   error_reporting(E_ALL);
   ini_set("display_errors", 1);
   require 'UserManager.php';

   $registerID = $_POST['registerID'];
   $registerPW = password_hash($_POST['registerPW'], PASSWORD_DEFAULT);
   $registerEmail = $_POST['registerEmail'];

   $userManager = new UserManager();

   $result = $userManager->Register($registerID, $registerPW, $registerEmail);

   echo json_encode($result, JSON_UNESCAPED_UNICODE | JSON_PRETTY_PRINT);
?>