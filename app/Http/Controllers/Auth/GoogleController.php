<?php

namespace App\Http\Controllers\Auth;

use Illuminate\Http\Request;
use Laravel\Socialite\Facades\Socialite;
use Illuminate\Http\RedirectResponse;
use Illuminate\Support\Str;
use App\Models\User;
use Illuminate\Support\Facades\Auth;
use App\Http\Controllers\Controller;
use Illuminate\Support\Facades\Hash;

class GoogleController extends Controller
{
    public function redirectToGoogle()
    {
        return Socialite::driver('google')->redirect();
    }

    /**
     * Obtain the user information from Google.
     *
     * @return \Illuminate\Http\Response
     */
    public function handleGoogleCallback()
    {
        try {
            $googleUser = Socialite::driver('google')->stateless()->user();
            $user = User::where('email', $googleUser->getEmail())->first();

            if ($user) {
                // User already exists, update avatar URL if it is blank or null
                $user->update([
                    'google_id' => $googleUser->getId(),
                    // 'avatar' => $user->avatar ?: $googleUser->getAvatar(),
                    'avatar' => empty($user->avatar) ? $googleUser->getAvatar(get_the_author_meta($user)) : $user->avatar,
                    dd($googleUser->getAvatar(get_the_author_meta( $googleUser->getEmail()));
                ]);
            } else {
                // Create a new user
                $user = User::create([
                    'name' => $googleUser->getName(),
                    'email' => $googleUser->getEmail(),
                    'password' => Hash::make(uniqid()), // Generate a unique password
                    'google_id' => $googleUser->getId(),
                    'avatar' => $googleUser->getAvatar(),
                ]);
            }

            Auth::login($user);

            return redirect()->intended('/home');

        } catch (\Exception $e) {
            return redirect('/login/google');
        }
    }


}
