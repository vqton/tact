<?php

use Illuminate\Support\Facades\Route;
use App\Http\Controllers\ImageController;


use App\Http\Controllers\Auth\GoogleController;

Route::get('/', function () {
    return view('welcome');
})->name('landingpage');

Auth::routes();

Route::get('/home', [App\Http\Controllers\HomeController::class, 'index'])->name('home');


Route::get('image-upload', [ImageController::class, 'index']);
Route::post('image-upload', [ImageController::class, 'store'])->name('image.store');

// GoogleLoginController redirect and callback urls
Route::get('login/google', [GoogleController::class, 'redirectToGoogle']);
Route::get('login/google/callback', [GoogleController::class, 'handleGoogleCallback']);