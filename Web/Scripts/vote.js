$("#confirmVoteButton").click(function () {
  $('#voteFailure').hide();
  $('#progressBar').show();

  var userId = $(this).attr("data-user-id");
  var supercarId = $(this).attr("data-supercar-id");
  var votes = $(this).attr("data-votes");
  var totalVotes = $(this).attr("data-total-votes");
  var firstName = $(this).attr("data-first-name");
  var lastName = $(this).attr("data-last-name");
  var comments = $('#comments').val();

  // Delay to allow progress bar to show in the UI.
  delay(function () {
    $.post('/api/vote', { userId: userId, supercarId: supercarId, comments: comments })
      .fail(function () {
        $('#progressBar').hide();
        $('#voteFailure').show();
      })
      .done(function () {
        $('#voteButton').hide();
        $('#voteContainer').html('Thank you for voting!');
        $("#voteContainer").addClass('alert alert-success');
        $('#voteModal').modal('hide');

        votes++;
        totalVotes++;

        $('#votesHeading').text(votes + ' ' + (votes == 1 ? 'vote' : 'votes') + ' out of ' + totalVotes + ' total');
        $('#voteBar').css('width', votes / totalVotes * 100 + '%');

        if (comments != '') {
          $('#results tr:last').after('<tr><td>' + firstName + ' ' + lastName + '</td><td>' + comments + '</td></tr>');
        }
      });
  }, 700);
});

$('#voteModal').on('hidden', function () {
  $('#voteFailure').hide();
});
