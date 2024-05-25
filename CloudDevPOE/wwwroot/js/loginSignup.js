function showLoginModalAndSubmitForm() {
	// Show the login modal
	var modal = $('#loginModal');
	modal.modal('show');

	// Close the modal when the backdrop is clicked
	modal.on('click', function (e) {
		if ($(e.target).is('.modal')) {
			modal.modal('hide');
		}
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
					// Login failed, display the error message
					$('#loginError').text(data.message);
				}
			}
		});
	});
}