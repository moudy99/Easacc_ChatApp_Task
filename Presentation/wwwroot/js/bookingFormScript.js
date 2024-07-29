document.addEventListener('DOMContentLoaded', function () {
    flatpickr('.flatpickr', {
        dateFormat: "Y-m-d",
        minDate: "today"
    });

    const bookingForm = document.getElementById('bookingForm');
    const validationSummary = document.querySelector('[data-valmsg-summary]');

    bookingForm.addEventListener('submit', function (event) {
        event.preventDefault();

        if (bookingForm.checkValidity()) {
            const formData = new FormData(bookingForm);

            const checkInDate = new Date(formData.get('CheckInDate'));
            const checkOutDate = new Date(formData.get('CheckOutDate'));
            if (checkInDate >= checkOutDate) {
                // Display validation error message in the validation summary
                validationSummary.textContent = 'Check-out date must be after check-in date';
                return;
            }
        } else {
            // Show error messages in validation summary
            validationSummary.textContent = 'Please correct the errors in the form.';
        }

        // Use AJAX to submit the form data
        const formData = new FormData(bookingForm);
        fetch(bookingForm.action, {
            method: 'POST',
            body: formData,
        })
            .then(response => {
                if (response.ok) {
                    // Show SweetAlert if the booking is successful
                    Swal.fire({
                        icon: 'success',
                        title: 'Booking Successful',
                        text: 'Your room has been booked successfully!',
                    });
                } else {
                    // Handle errors if the booking is not successful
                    Swal.fire({
                        icon: 'error',
                        title: 'Oops...',
                        text: 'Something went wrong!',
                    });
                }
            })
            .catch(error => {
                // Handle network errors
                Swal.fire({
                    icon: 'error',
                    title: 'Oops...',
                    text: 'An error occurred: ' + error.message,
                });
            });
    });

    document.getElementById('nextButton').addEventListener('click', function () {
        const numberOfRooms = document.getElementById('numberOfRooms').value;
        if (numberOfRooms > 0) {
            const roomDetailsContainer = document.getElementById('roomDetailsContainer');
            roomDetailsContainer.innerHTML = '';
            for (let i = 0; i < numberOfRooms; i++) {
                const roomDetail = document.createElement('div');
                roomDetail.className = 'room-detail mb-3';

                // Create a title for the room
                const roomTitle = document.createElement('h4');
                roomTitle.textContent = `Room ${i + 1} Details`;
                roomDetail.appendChild(roomTitle);

                // Create form elements for room details
                roomDetail.innerHTML += `
                                <div class="form-group mb-3">
                                    <label>Room Type</label>
                                    <select class="form-control" name="RoomBookings[${i}].RoomType">
                                        <option value="Single">Single</option>
                                        <option value="Double">Double</option>
                                        <option value="Suite">Suite</option>
                                    </select>
                                </div>
                                <div class="form-group mb-3">
                                    <label>Number of Adults</label>
                                    <input class="form-control" type="number" name="RoomBookings[${i}].NumberOfAdults" />
                                </div>
                                <div class="form-group mb-3">
                                    <label>Number of Children</label>
                                    <input class="form-control" type="number" name="RoomBookings[${i}].NumberOfChildren" />
                                </div>
                            `;
                roomDetailsContainer.appendChild(roomDetail);
            }
            document.getElementById('step1').style.display = 'none';
            document.getElementById('step2').style.display = 'block';
        } else {
            alert('Please enter a valid number of rooms');
        }
    });

    document.getElementById('prevButton').addEventListener('click', function () {
        document.getElementById('step2').style.display = 'none';
        document.getElementById('step1').style.display = 'block';
    });
});