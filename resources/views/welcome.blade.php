@extends('layouts.app')
@section('content')
    <!-- Hero Section -->
    <section class="hero">
        <div class="container">
            <h1>Simplify Your Accounting</h1>
            <p>Manage your finances effortlessly with our comprehensive accounting solution.</p>
            <a href="#features" class="btn btn-primary btn-lg">Learn More</a>
        </div>
    </section>
    <!-- Features Section -->
    <section id="features" class="features">
        <div class="container">
            <div class="row text-center">
                <div class="col-lg-4 mb-4">
                    <img src="{{ asset('images/report.jpg') }}" class="img-fluid mb-3" alt="Financial Reporting"
                        height="150" width="150">
                    <h3>Financial Reporting</h3>
                    <p>Generate detailed financial reports to keep track of your business performance.</p>
                </div>
                <div class="col-lg-4 mb-4">
                    <img src="{{ asset('images/node_tracking.jpg') }}" class="img-fluid mb-3" alt="Expense Tracking"
                        height="150" width="150">
                    <h3>Expense Tracking</h3>
                    <p>Monitor and categorize your expenses to optimize your spending.</p>
                </div>
                <div class="col-lg-4 mb-4">
                    <img src="{{ asset('images/client_mng.jpg') }}" class="img-fluid mb-3" alt="Client Management"
                        height="150" width="150">
                    <h3>Client Management</h3>
                    <p>Manage your client information and billing with ease.</p>
                </div>
            </div>
        </div>
    </section>

    <!-- How It Works Section -->
    <section id="how-it-works" class="py-5">
        <div class="container">
            <div class="row text-center">
                <div class="col-lg-12 mb-4">
                    <h2>How It Works</h2>
                </div>
                <div class="col-lg-4">
                    <img src="{{ asset('images/sign-up-icon-29418.png') }}" class="img-fluid mb-3" alt="Sign Up"
                        width="100" height="100">
                    <h4>1. Sign Up</h4>
                    <p>Create an account and set up your business profile in minutes.</p>
                </div>
                <div class="col-lg-4">
                    <img src="{{ asset('images/add_trans.jpg') }}" class="img-fluid mb-3" alt="Add Transactions"
                        width="100" height="100">
                    <h4>2. Add Transactions</h4>
                    <p>Record your income and expenses with our user-friendly interface.</p>
                </div>
                <div class="col-lg-4">
                    <img src="{{ asset('images/generateReport.jpg') }}" class="img-fluid mb-3" alt="Generate Reports"
                        width="100" height="100">
                    <h4>3. Generate Reports</h4>
                    <p>Get instant insights into your financial health with customizable reports.</p>
                </div>
            </div>
        </div>
    </section>
    <section id="pricing" class="pricing">
        <div class="container">
            <div class="row text-center">
                <div class="col-lg-12 mb-4">
                    <h2>Explore Pricing</h2>
                </div>
                <div class="col-lg-4 mb-4">
                    <div class="card bg-success text-white">
                        <img src="{{ asset('images/basic.svg') }}" class="card-img-top" alt="Basic Plan">

                        <div class="card-body">
                            {{-- <h5 class="card-title">Basic Plan</h5>
                            <h6 class="card-subtitle mb-2 text-muted">$9.99/month</h6> --}}
                            <ul class="list-unstyled">
                                <li>Essential Services</li>
                                <li>Invoicing</li>
                                <li>Expense Tracking</li>
                                <li>Limited Support</li>
                                <li>Mobile App</li>
                            </ul>
                            <a href="#" class="btn btn-primary">Get Started</a>
                        </div>
                    </div>
                </div>
                <div class="col-lg-4 mb-4">
                    <div class="card">
                        <img src="{{ asset('images/standard.svg') }}" class="card-img-top"alt="Standard Plan">
                        <div class="card-body bg-warning text-white text-left">
                            {{-- <h5 class="card-title">Standard Plan</h5>
                            <h6 class="card-subtitle mb-2 text-muted">$19.99/month</h6> --}}
                            <ul class="list-unstyled text-left">
                                <li>Expanded Services</li>
                                <li>Payroll</li>
                                <li>Advanced Reporting</li>
                                <li>Dedicated Support</li>
                                <li>Integration</li>
                            </ul>
                            <a href="#" class="btn btn-primary">Get Started</a>
                        </div>
                    </div>
                </div>
                <div class="col-lg-4 mb-4">
                    <div class="card">
                        <img src="{{ asset('images/premium.svg') }}" class="card-img-top" alt="Premium Plan">
                        <div class="card-body bg-danger text-white">
                            {{-- <h5 class="card-title">Premium Plan</h5>
                            <h6 class="card-subtitle mb-2 text-muted">$29.99/month</h6> --}}
                            <ul class="list-unstyled">
                                <li>Customization</li>
                                <li>Advanced Features</li>
                                <li>Unlimited Usage</li>
                                <li>Dedicated Account Manager</li>
                                <li>Tax Advisory</li>
                            </ul>
                            <a href="#" class="btn btn-primary">Get Started</a>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </section>


    <!-- Testimonials Section -->
    <section id="testimonials" class="testimonials">
        <div class="container">
            <div class="row text-center">
                <div class="col-lg-12 mb-4">
                    <h2>What Our Customers Say</h2>
                </div>
                <div class="col-lg-4 mb-4">
                    <div class="testimonial-card">
                        <p class="testimonial-text">"This accounting app has transformed my business. Highly recommended!"
                        </p>
                        <footer class="testimonial-author">John Doe, <cite title="Company A">Company A</cite></footer>
                    </div>
                </div>
                <div class="col-lg-4 mb-4">
                    <div class="testimonial-card">
                        <p class="testimonial-text">"Excellent features and easy to use. A must-have for any business."</p>
                        <footer class="testimonial-author">Jane Smith, <cite title="Company B">Company B</cite></footer>
                    </div>
                </div>
                <div class="col-lg-4 mb-4">
                    <div class="testimonial-card">
                        <p class="testimonial-text">"Great value for money. The customer support is top-notch."</p>
                        <footer class="testimonial-author">Mike Johnson, <cite title="Company C">Company C</cite></footer>
                    </div>
                </div>
            </div>
        </div>
    </section>


    <!-- Contact Section -->
    <section id="contact" class="contact">
        <div class="container">
            <div class="row text-center">
                <div class="col-lg-12 mb-4">
                    <h2>Contact Us</h2>
                </div>
                <div class="col-lg-6 offset-lg-3">
                    <form>
                        <div class="mb-3">
                            <input type="email" class="form-control" id="email" placeholder="Your Email">
                        </div>
                        <div class="mb-3">
                            <textarea class="form-control" id="message" rows="3" placeholder="Your Message"></textarea>
                        </div>
                        <button type="submit" class="btn btn-primary">Send</button>
                    </form>
                </div>
            </div>
        </div>
    </section>
@endsection
