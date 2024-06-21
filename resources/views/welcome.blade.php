@extends('layouts.app')
@section('content')
    
<header class="bg-primary text-white text-center py-5">
    <div class="container">
        <h1>Welcome to Accounting App</h1>
        <p class="lead">Your all-in-one accounting solution</p>
        <a href="#features" class="btn btn-secondary btn-lg">Learn More</a>
    </div>
</header>

<section id="features" class="py-5">
    <div class="container">
        <div class="row text-center">
            <div class="col-md-4">
                <div class="feature-icon bg-primary text-white mb-3"><i class="bi bi-graph-up"></i></div>
                <h3>Financial Reports</h3>
                <p>Generate detailed financial reports and insights.</p>
            </div>
            <div class="col-md-4">
                <div class="feature-icon bg-primary text-white mb-3"><i class="bi bi-wallet"></i></div>
                <h3>Expense Tracking</h3>
                <p>Track all your expenses effortlessly.</p>
            </div>
            <div class="col-md-4">
                <div class="feature-icon bg-primary text-white mb-3"><i class="bi bi-people"></i></div>
                <h3>Multi-User Support</h3>
                <p>Collaborate with your team in real-time.</p>
            </div>
        </div>
    </div>
</section>

<section id="pricing" class="bg-light py-5">
    <div class="container text-center">
        <h2>Pricing</h2>
        <p class="lead">Choose a plan that fits your needs</p>
        <div class="row">
            <div class="col-md-4">
                <div class="card mb-4">
                    <div class="card-header">
                        <h4 class="my-0">Free</h4>
                    </div>
                    <div class="card-body">
                        <h1 class="card-title pricing-card-title">$0 <small class="text-muted">/ mo</small></h1>
                        <ul class="list-unstyled mt-3 mb-4">
                            <li>Basic features</li>
                            <li>Single user</li>
                            <li>Email support</li>
                        </ul>
                        <button type="button" class="w-100 btn btn-lg btn-outline-primary">Sign up for free</button>
                    </div>
                </div>
            </div>
            <div class="col-md-4">
                <div class="card mb-4">
                    <div class="card-header">
                        <h4 class="my-0">Pro</h4>
                    </div>
                    <div class="card-body">
                        <h1 class="card-title pricing-card-title">$15 <small class="text-muted">/ mo</small></h1>
                        <ul class="list-unstyled mt-3 mb-4">
                            <li>Advanced features</li>
                            <li>Up to 10 users</li>
                            <li>Priority support</li>
                        </ul>
                        <button type="button" class="w-100 btn btn-lg btn-primary">Get started</button>
                    </div>
                </div>
            </div>
            <div class="col-md-4">
                <div class="card mb-4">
                    <div class="card-header">
                        <h4 class="my-0">Enterprise</h4>
                    </div>
                    <div class="card-body">
                        <h1 class="card-title pricing-card-title">$30 <small class="text-muted">/ mo</small></h1>
                        <ul class="list-unstyled mt-3 mb-4">
                            <li>All features</li>
                            <li>Unlimited users</li>
                            <li>Dedicated support</li>
                        </ul>
                        <button type="button" class="w-100 btn btn-lg btn-primary">Contact us</button>
                    </div>
                </div>
            </div>
        </div>
    </div>
</section>

<section id="contact" class="py-5">
    <div class="container text-center">
        <h2>Contact Us</h2>
        <p class="lead">We'd love to hear from you</p>
        <form>
            <div class="row">
                <div class="col-md-6">
                    <input type="text" class="form-control mb-3" placeholder="Your Name">
                </div>
                <div class="col-md-6">
                    <input type="email" class="form-control mb-3" placeholder="Your Email">
                </div>
            </div>
            <textarea class="form-control mb-3" rows="5" placeholder="Your Message"></textarea>
            <button type="submit" class="btn btn-primary btn-lg">Send Message</button>
        </form>
    </div>
</section>
@endsection


