$(function() {
    var delay = 250; // 250 millisecond delay
    var dancerTimer;
    var dancerJqxhr;
    
    function requestDancerData()
    {
        searchForDancer($("#dancerQuery").val(), 0);
    };
    
    $("#dancerQuery").on(
        "input",
        function() {
            clearTimeout(dancerTimer);
            dancerTimer = setTimeout(requestDancerData, delay);

            // abort any previous request which is in progress
            if (dancerJqxhr)
            {
                dancerJqxhr.abort();
                dancerJqxhr = null;
            }
        }
    );
    
    $("#findDancerLink").on(
        "click",
        function() {
            $("#findDancerPanel").toggle();
            
            if ($("#findDancerPanel").is(":visible"))
            {
                $("#dancerQuery").focus();
            }
        }
    );

    $("#clearDancerLink").on(
        "click",
        function() {
            $("#dancerNameList").text("(None)");
            $("#dancerIdList").val("");
        }
    );
    
    $("#dancerSearchResults").on(
        "click",
        ".dancerResult",
        function(evt) {
            var link = $(evt.target);
            var dancerId = link.attr("data-wsdc-id");
            var dancerName = link.attr("data-name");
            addDancer(dancerId, dancerName);
        }
    );
    
    $("#dancerSearchResults").on(
        "click",
        ".pagingLink",
        function(evt) {
            var link = $(evt.target);
            var query = link.attr("data-query");
            var start = link.attr("data-start");
            searchForDancer(query, start);
        }
    );
    
    function addDancer(dancerId, dancerName)
    {
        var dancerIds = $("#dancerIdList").val();
        var dancerNames = $("#dancerNameList").text();
        
        if (dancerIds && dancerIds.length)
        {
            if (dancerIds.charAt(dancerIds.length - 1) != ';')
            {
                dancerIds += ";";
                dancerNames += "; ";
            }
            
            dancerIds += dancerId;
            dancerNames += dancerName;
        }
        else
        {
            dancerIds = dancerId;
            dancerNames = dancerName;
        }
        
        $("#dancerIdList").val(dancerIds);
        $("#dancerNameList").text(dancerNames);
        $("#findDancerPanel").hide();
        $("#dancerQuery").val("");
        $("#dancerSearchResults").html("");
    };
    
    function searchForDancer(query, start)
    {
        dancerJqxhr = $.ajax({
            method: "GET",
            url: "/Videos/DancerSearchResults",
            data: { query: query, start: start }
        });
        
        dancerJqxhr.done(
            function(data) {
                $("#dancerSearchResults").html(data);
            });
    }
});
