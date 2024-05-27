/*LoginSignup Javascript file*/
function showLoginModalAndSubmitForm() {
	// Show the login modal
	var modal = $('#loginModal');
	modal.modal('show');

	// Close the modal when the backdrop is clicked
	modal.on('click', function (e) {
		if ($(e.target).is('.modal')) {
			console.log('Modal is about to be hidden');
			modal.modal('hide');
		}
	});

	// Hide the error bar when the user starts typing
	$('input[name="email"], input[name="password"]').on('input', function () {
		$('#errorBar').hide();
	});

	// Submit the login form
	$('#loginForm').off('submit').on('submit', function (e) {
		e.preventDefault();

		var email = $('input[name="email"]').val();
		var password = $('input[name="password"]').val();

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
}