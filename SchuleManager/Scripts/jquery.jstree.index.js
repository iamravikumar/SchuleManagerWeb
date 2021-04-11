/// <reference path="jquery-1.7.1-vsdoc.js" />

$(function () {
    $("#moduleTree").jstree({
        json_data: {
            data: treeModel,
            expand_selected_onload: false
        },
        checkbox: {
            real_checkboxes: true,
            real_checkboxes_names: function(n) {
                return [("check_" + (n[0].id || Math.ceil(Math.random() * 10000))), n[0].id];
            }
        },
        plugins: ["themes", "json_data", "ui", "checkbox"]
    }).on("loaded.jstree select_node.jstree", function (event, data) {
        $('#moduleTree').jstree('check_node', 'li[selected=selected]');
    });
});