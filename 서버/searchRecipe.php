<?php
   header( "Content-Type:application/json;charset=UTF-8" );

   error_reporting(E_ALL);
   ini_set("display_errors", 1);
   require 'RecipeManager.php';

   $search_keyword = $_POST['searchKeyword'];
   $search_option = $_POST['searchOption'];

   $recipeManager= new RecipeManager();

   $recipeManager->SearchRecipe($search_keyword, $search_option);
?>