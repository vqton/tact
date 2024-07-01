<!doctype html>
<html lang="{{ str_replace('_', '-', app()->getLocale()) }}">

<head>
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1">

    <!-- CSRF Token -->
    <meta name="csrf-token" content="{{ csrf_token() }}">

    <title>{{ config('app.name', 'Laravel') }}</title>

    <!-- Fonts -->
    <link rel="dns-prefetch" href="//fonts.bunny.net">
    <link href="https://fonts.bunny.net/css?family=Nunito" rel="stylesheet">


    <!-- Scripts -->
    @vite(['resources/css/bootstrap.css', 'resources/js/app.js'])

    <style>
        body {
            font-family: Arial, sans-serif;
            margin: 0;
            padding: 0;
        }

        .hero {
            background: url({{ asset('images/hero_page2.jpg') }}) no-repeat center center;
            background-size: cover;
            color: white;
            text-align: center;
            padding: 150px 0;
            position: relative;
        }

        .hero::before {
            content: "";
            position: absolute;
            top: 0;
            left: 0;
            width: 100%;
            height: 100%;
            background-color: rgba(0, 0, 0, 0.5);
            /* Add a subtle overlay */
        }

        .hero h1 {
            font-size: 3rem;
            font-weight: bold;
            margin-bottom: 20px;
        }

        .hero p {
            font-size: 1.25rem;
            margin-bottom: 30px;
        }

        /* Other sections (features, pricing, etc.) go here */

        .testimonials {
            background-color: #f1f1f1;
            padding: 60px 0;
        }

        /* Add styling for testimonials as discussed earlier */

        .footer {
            background-color: #f9f9f9;
            color: #343a40;
            text-align: center;
            padding: 20px 0;
        }

        .footer p {
            margin: 0;
        }
    </style>
</head>

<body>

    <!-- Navbar -->
    <nav class="navbar navbar-expand-lg navbar-light bg-light">
        <div class="container">
            <a class="navbar-brand" href="{{ route('landingpage') }}">{{ config('app.name', 'Laravel') }}</a>
            <button class="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#navbarNav"
                aria-controls="navbarNav" aria-expanded="false" aria-label="Toggle navigation">
                <span class="navbar-toggler-icon"></span>
            </button>
            <div class="collapse navbar-collapse" id="navbarNav">
                <ul class="navbar-nav ms-auto">
                    <li class="nav-item"><a class="nav-link" href="{{ route('landingpage') }}#features">Features</a>
                    </li>
                    <li class="nav-item"><a class="nav-link" href="{{ route('landingpage') }}#how-it-works">How It
                            Works</a></li>
                    <li class="nav-item"><a class="nav-link" href="{{ route('landingpage') }}#pricing">Pricing</a></li>
                    <li class="nav-item"><a class="nav-link"
                            href="{{ route('landingpage') }}#testimonials">Testimonials</a></li>
                    <li class="nav-item"><a class="nav-link" href="{{ route('landingpage') }}#contact">Contact</a></li>
                    @guest
                        <li class="nav-item"><a class="nav-link" href="{{ route('login') }}">Login</a></li>
                        <li class="nav-item"><a class="nav-link" href="{{ route('register') }}">Sign Up</a></li>
                    @else
                        <li class="nav-item">
                            <img src="{{ Auth::user()->avatar }}" alt="User Avatar" class="rounded-circle" width="40"
                                height="40">
                        </li>
                        <li class="nav-item">
                            <a class="nav-link" href="#">{{ Auth::user()->name }}</a>
                        </li>
                        <li class="nav-item">
                            <a class="nav-link" href="#"
                                onclick="event.preventDefault(); document.getElementById('logout-form').submit();">Logout</a>
                            <form id="logout-form" action="{{ route('logout') }}" method="POST" style="display: none;">
                                @csrf
                            </form>
                        </li>
                    @endguest
                </ul>
            </div>
        </div>
    </nav>

    <main class="py-4">
        @yield('content')
    </main>


    <!-- Footer -->
    <footer class="footer py-4 text-center">
        <div class="container">
            <p>&copy; 2024 {{ config('app.name', 'Laravel') }}. All Rights Reserved.</p>
        </div>
    </footer>
</body>
<script src="https://cdn.jsdelivr.net/npm/@popperjs/core@2.11.6/dist/umd/popper.min.js"></script>
<script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/js/bootstrap.min.js"></script>

</html>
