/*LoginSignup Javascript file*/
function showLoginModalAndSubmitForm() {
	// Show the login modal
	var modal = $('#authModal');
	modal.modal('show');

	//<><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><>//
	// Close the modal when the backdrop is clicked
	modal.on('click', function (e) {
		if ($(e.target).is('.modal')) {
			console.log('Modal is about to be hidden');
			modal.modal('hide');
		}
	});

	//<><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><>//
	// Hide the error bar when the user starts typing in the login form
	$('input[name="login-email"], input[name="login-password"]').on('input', function () {
		$('#errorBar').hide();
	});

	//<><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><>//
	// Live validation for the signup form
	$('input[name="signup-name"], input[name="signup-surname"]').on('input', function () {
		var input = $(this).val();
		if (input.length > 50) {
			$('#errorBar' + $(this).attr('name').split('-')[1].charAt(0).toUpperCase() + $(this).attr('name').split('-')[1].slice(1)).text("Cannot be longer than 50 characters.").show();
		} else if (input.length === 0) {
			$('#errorBar' + $(this).attr('name').split('-')[1].charAt(0).toUpperCase() + $(this).attr('name').split('-')[1].slice(1)).hide();
		} else {
			$('#errorBar' + $(this).attr('name').split('-')[1].charAt(0).toUpperCase() + $(this).attr('name').split('-')[1].slice(1)).hide();
		}
	});

	//--------------------------------------------------------------------------------------------------------------------------//
	var emailErrorShown = false;
	$('input[name="signup-email"]').on('focusout', function () {
		var email = $(this).val();
		if (email.length > 255) {
			$('#errorBarEmail').text("Email cannot be longer than 255 characters.").show();
			emailErrorShown = true;
		} else if (!/^[\w-]+(\.[\w-]+)*@([\w-]+\.)+[a-zA-Z]{2,7}$/.test(email) && email.length !== 0) {
			$('#errorBarEmail').text("Please enter a valid email address.").show();
			emailErrorShown = true;
		}
	}).on('input', function () {
		if (emailErrorShown && /^[\w-]+(\.[\w-]+)*@([\w-]+\.)+[a-zA-Z]{2,7}$/.test($(this).val())) {
			$('#errorBarEmail').hide();
			emailErrorShown = false;
		}
	});

	//--------------------------------------------------------------------------------------------------------------------------//
	var passwordErrorShown = false;
	$('input[name="signup-password"]').on('focusout', function () {
		var password = $(this).val();
		if (password.length > 128) {
			$('#errorBarPassword').text("Password cannot be longer than 128 characters.").show();
			passwordErrorShown = true;
		} else if (!/^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$/.test(password) && password.length !== 0) {
			$('#errorBarPassword').text("Password must be at least 8 characters and contain at least 1 uppercase letter, 1 lowercase letter, 1 digit, and 1 special character.").show();
			passwordErrorShown = true;
		}
	}).on('input', function () {
		if (passwordErrorShown && /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$/.test($(this).val())) {
			$('#errorBarPassword').hide();
			passwordErrorShown = false;
		}
	});

	//--------------------------------------------------------------------------------------------------------------------------//
	var confirmPasswordErrorShown = false;
	$('input[name="signup-confirmPassword"]').on('focusout', function () {
		var confirmPassword = $(this).val();
		var password = $('input[name="signup-password"]').val();
		if (password !== confirmPassword && confirmPassword.length !== 0) {
			$('#errorBarConfirmPassword').text("Passwords do not match.").show();
			confirmPasswordErrorShown = true;
		}
	}).on('input', function () {
		if (confirmPasswordErrorShown && $(this).val() === $('input[name="signup-password"]').val()) {
			$('#errorBarConfirmPassword').hide();
			confirmPasswordErrorShown = false;
		}
	});

	//<><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><>//
	// Switch to the login form
	$('#switchToLogin').on('click', function () {
		$('#signupForm').hide();
		$('#loginForm').show();
	});

	//--------------------------------------------------------------------------------------------------------------------------//
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

	//<><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><>//
	// Switch to the signup form
	$('#switchToSignup').on('click', function () {
		$('#loginForm').hide();
		$('#signupForm').show();
	});

	//--------------------------------------------------------------------------------------------------------------------------//
	// Submit the signup form
	$('#signupForm').off('submit').on('submit', function (e) {
		e.preventDefault();

		var name = $('input[name="signup-name"]').val();
		var surname = $('input[name="signup-surname"]').val();
		var email = $('input[name="signup-email"]').val();
		var password = $('input[name="signup-password"]').val();
		var confirmPassword = $('input[name="signup-confirmPassword"]').val();

		// Validate the input
		if (name.length === 0 || surname.length === 0 || email.length === 0 || password.length === 0 || confirmPassword.length === 0) {
			$('#errorBarSignup').text("Fields cannot be blank.").show();
			return;
		}
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
					// Signup was successful, show the SweetAlert2 confirmation message
					Swal.fire({
						icon: 'success',
						title: 'Signup successful',
						text: 'You have been signed up and logged in successfully.',
					}).then((result) => {
						// Refresh the page after the user closes the SweetAlert2 message
						if (result.isConfirmed) {
							location.reload();
						}
					});
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
	//<><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><>//
}