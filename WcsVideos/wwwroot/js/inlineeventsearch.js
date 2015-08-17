$(function() {
    var delay = 250; // 250 millisecond delay
    var eventTimer;
    var eventJqxhr;

    function requestEventData()
    {
        searchForEvent($("#eventQuery").val(), 0);
    };
                
    $("#eventQuery").on(
        "input",
        function() {
            clearTimeout(eventTimer);
            eventTimer = setTimeout(requestEventData, delay);

            // abort any previous request which is in progress
            if (eventJqxhr)
            {
                eventJqxhr.abort();
                eventJqxhr = null;
            }
        }
    );
    
    $("#findEventLink").on(
        "click",
        function() {
            $("#findEventPanel").toggle();
                                
            if ($("#findEventPanel").is(":visible"))
            {
                $("#eventQuery").focus();
            }
        }
    );
    
    $("#clearEventLink").on(
        "click",
        function() {
            setEvent("", "(None)");
        }
    );
    
    $("#eventSearchResults").on(
        "click",
        ".eventResult",
        function(evt) {
            var link = $(evt.target);
            var eventId = link.attr("data-event-id");
            var eventName = link.attr("data-name");
            setEvent(eventId, eventName);
        }
    );
    
    $("#eventSearchResults").on(
        "click",
        ".pagingLink",
        function(evt) {
            var link = $(evt.target);
            var query = link.attr("data-query");
            var start = link.attr("data-start");
            searchForEvent(query, start);
        }
    );
    
    function setEvent(eventId, eventName)
    {
        $("#eventId").val(eventId);
        $("#eventName").text(eventName);
        $("#findEventPanel").hide();
        $("#eventQuery").val("");
        $("#eventSearchResults").html("");
    };
    
    function searchForEvent(query, start)
    {
        eventJqxhr = $.ajax({
            method: "GET",
            url: "/Videos/EventSearchResults",
            data: { query: query, start: start }
        });
        
        eventJqxhr.done(
            function(data) {
                $("#eventSearchResults").html(data);
            });
    }
});