<?php

use Illuminate\Support\Facades\Route;
use App\Http\Controllers\Auth\LoginController;

Route::get('/', function () {
    return view('welcome');
});
Route::get('/dashboard', function () {
    return view('dashboard');
});

Auth::routes();
Route::get('/login/google', [LoginController::class, 'redirectToGoogleProvider'])->name('login.google');
Route::get('/login/google/callback', [LoginController::class, 'handleGoogleProviderCallback']);
Route::get('/home', [App\Http\Controllers\HomeController::class, 'index'])->name('home');
