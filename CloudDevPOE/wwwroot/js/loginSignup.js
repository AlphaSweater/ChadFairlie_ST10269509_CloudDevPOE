/*LoginSignup Javascript file*/
function showLoginModalAndSubmitForm() {
	// Show the login modal
	var modal = $('#authModal');
	modal.modal('show');

	// Close the modal when the backdrop is clicked
	modal.on('click', function (e) {
		if ($(e.target).is('.modal')) {
			console.log('Modal is about to be hidden');
			modal.modal('hide');
		}
	});

	// Hide the error bar when the user starts typing in the login form
	$('input[name="login-email"], input[name="login-password"]').on('input', function () {
		$('#errorBar').hide();
	});

	// Hide the error bar when the user starts typing in the signup form
	$('input[name="signup-name"], input[name="signup-surname"], input[name="signup-email"], input[name="signup-password"], input[name="signup-confirmPassword"]').on('input', function () {
		$('#errorBarSignup').hide();
	});

	// Switch to the signup form
	$('#switchToSignup').on('click', function () {
		$('#loginForm').hide();
		$('#signupForm').show();
	});

	// Switch to the login form
	$('#switchToLogin').on('click', function () {
		$('#signupForm').hide();
		$('#loginForm').show();
	});

	// Submit the login form
	$('#loginForm').off('submit').on('submit', function (e) {
		e.preventDefault();

		var email = $('input[name="login-email"]').val();
		var password = $('input[name="login-password"]').val();

		$.ajax({
			url: "/User/Login",
			type: "POST",
			data: { email: email, password: password },
			success: function (data) {
				if (data.success) {
					// Login was successful, refresh the page
					location.reload();
				} else {
					// Show the error message in the error bar
					$('#errorBar').text(data.message).show();
				}
			},
			error: function () {
				// Show a generic error message
				$('#errorBar').text("An error occurred while processing your request.").show();
			}
		});
	});

	// Submit the signup form
	$('#signupForm').off('submit').on('submit', function (e) {
		e.preventDefault();

		var name = $('input[name="signup-name"]').val();
		var surname = $('input[name="signup-surname"]').val();
		var email = $('input[name="signup-email"]').val();
		var password = $('input[name="signup-password"]').val();
		var confirmPassword = $('input[name="signup-confirmPassword"]').val();

		// Validate the input
		if (name.length > 50) {
			$('#errorBarSignup').text("Name cannot be longer than 50 characters.").show();
			return;
		}
		if (surname.length > 50) {
			$('#errorBarSignup').text("Surname cannot be longer than 50 characters.").show();
			return;
		}
		if (email.length > 255) {
			$('#errorBarSignup').text("Email cannot be longer than 255 characters.").show();
			return;
		}
		if (!/^[\w-]+(\.[\w-]+)*@([\w-]+\.)+[a-zA-Z]{2,7}$/.test(email)) {
			$('#errorBarSignup').text("Invalid Email Address.").show();
			return;
		}
		if (password.length > 128) {
			$('#errorBarSignup').text("Password cannot be longer than 128 characters.").show();
			return;
		}
		if (!/^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$/.test(password)) {
			$('#errorBarSignup').text("Password must be at least 8 characters and contain at least 1 uppercase letter, 1 lowercase letter, 1 digit, and 1 special character.").show();
			return;
		}
		if (password !== confirmPassword) {
			$('#errorBarSignup').text("Passwords do not match.").show();
			return;
		}

		// If the input is valid, send the AJAX request
		$.ajax({
			url: "/User/SignUp",
			type: "POST",
			data: { name: name, surname: surname, email: email, password: password },
			success: function (data) {
				if (data.success) {
					// Signup was successful, refresh the page
					location.reload();
				} else {
					// Show the error message in the error bar
					$('#errorBarSignup').text(data.message).show();
				}
			},
			error: function () {
				// Show a generic error message
				$('#errorBarSignup').text("An error occurred while processing your request.").show();
			}
		});
	});
}