﻿/*!
   Copyright 2017-2019 SpryMedia Ltd.

 This source file is free software, available under the following license:
   MIT license - http://datatables.net/license/mit

 This source file is distributed in the hope that it will be useful, but
 WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
 or FITNESS FOR A PARTICULAR PURPOSE. See the license files for details.

 For details please refer to: http://www.datatables.net
 RowGroup 1.1.1
 ©2017-2019 SpryMedia Ltd - datatables.net/license
*/
var $jscomp = $jscomp || {}; $jscomp.scope = {}; $jscomp.findInternal = function (a, d, c) { a instanceof String && (a = String(a)); for (var e = a.length, f = 0; f < e; f++) { var h = a[f]; if (d.call(c, h, f, a)) return { i: f, v: h } } return { i: -1, v: void 0 } }; $jscomp.ASSUME_ES5 = !1; $jscomp.ASSUME_NO_NATIVE_MAP = !1; $jscomp.ASSUME_NO_NATIVE_SET = !1; $jscomp.SIMPLE_FROUND_POLYFILL = !1;
$jscomp.defineProperty = $jscomp.ASSUME_ES5 || "function" == typeof Object.defineProperties ? Object.defineProperty : function (a, d, c) { a != Array.prototype && a != Object.prototype && (a[d] = c.value) }; $jscomp.getGlobal = function (a) { return "undefined" != typeof window && window === a ? a : "undefined" != typeof global && null != global ? global : a }; $jscomp.global = $jscomp.getGlobal(this);
$jscomp.polyfill = function (a, d, c, e) { if (d) { c = $jscomp.global; a = a.split("."); for (e = 0; e < a.length - 1; e++) { var f = a[e]; f in c || (c[f] = {}); c = c[f] } a = a[a.length - 1]; e = c[a]; d = d(e); d != e && null != d && $jscomp.defineProperty(c, a, { configurable: !0, writable: !0, value: d }) } }; $jscomp.polyfill("Array.prototype.find", function (a) { return a ? a : function (a, c) { return $jscomp.findInternal(this, a, c).v } }, "es6", "es3");
(function (a) { "function" === typeof define && define.amd ? define(["jquery", "datatables.net"], function (d) { return a(d, window, document) }) : "object" === typeof exports ? module.exports = function (d, c) { d || (d = window); c && c.fn.dataTable || (c = require("datatables.net")(d, c).$); return a(c, d, d.document) } : a(jQuery, window, document) })(function (a, d, c, e) {
    var f = a.fn.dataTable, h = function (b, g) {
        if (!f.versionCheck || !f.versionCheck("1.10.8")) throw "RowGroup requires DataTables 1.10.8 or newer"; this.c = a.extend(!0, {}, f.defaults.rowGroup,
            h.defaults, g); this.s = { dt: new f.Api(b) }; this.dom = {}; b = this.s.dt.settings()[0]; if (g = b.rowGroup) return g; b.rowGroup = this; this._constructor()
    }; a.extend(h.prototype, {
        dataSrc: function (b) { if (b === e) return this.c.dataSrc; var g = this.s.dt; this.c.dataSrc = b; a(g.table().node()).triggerHandler("rowgroup-datasrc.dt", [g, b]); return this }, disable: function () { this.c.enable = !1; return this }, enable: function (b) { if (!1 === b) return this.disable(); this.c.enable = !0; return this }, _constructor: function () {
            var b = this, a = this.s.dt; a.on("draw.dtrg",
                function () { b.c.enable && b._draw() }); a.on("column-visibility.dt.dtrg responsive-resize.dt.dtrg", function () { b._adjustColspan() }); a.on("destroy", function () { a.off(".dtrg") }); a.on("responsive-resize.dt", function () { b._adjustColspan() })
        }, _adjustColspan: function () { a("tr." + this.c.className, this.s.dt.table().body()).find("td").attr("colspan", this._colspan()) }, _colspan: function () { return this.s.dt.columns().visible().reduce(function (b, a) { return b + a }, 0) }, _draw: function () {
            var b = this._group(0, this.s.dt.rows({ page: "current" }).indexes());
            this._groupDisplay(0, b)
        }, _group: function (b, g) { for (var c = a.isArray(this.c.dataSrc) ? this.c.dataSrc : [this.c.dataSrc], d = f.ext.oApi._fnGetObjectDataFn(c[b]), h = this.s.dt, l, n, m = [], k = 0, p = g.length; k < p; k++) { var q = g[k]; l = h.row(q).data(); l = d(l); if (null === l || l === e) l = this.c.emptyDataGroup; if (n === e || l !== n) m.push({ dataPoint: l, rows: [] }), n = l; m[m.length - 1].rows.push(q) } if (c[b + 1] !== e) for (k = 0, p = m.length; k < p; k++)m[k].children = this._group(b + 1, m[k].rows); return m }, _groupDisplay: function (b, a) {
            for (var c = this.s.dt, g, d = 0, f =
                a.length; d < f; d++) { var e = a[d], h = e.dataPoint, k = e.rows; this.c.startRender && (g = this.c.startRender.call(this, c.rows(k), h, b), (g = this._rowWrap(g, this.c.startClassName, b)) && g.insertBefore(c.row(k[0]).node())); this.c.endRender && (g = this.c.endRender.call(this, c.rows(k), h, b), (g = this._rowWrap(g, this.c.endClassName, b)) && g.insertAfter(c.row(k[k.length - 1]).node())); e.children && this._groupDisplay(b + 1, e.children) }
        }, _rowWrap: function (b, g, c) {
            if (null === b || "" === b) b = this.c.emptyDataGroup; return b === e || null === b ? null : ("object" ===
                typeof b && b.nodeName && "tr" === b.nodeName.toLowerCase() ? a(b) : b instanceof a && b.length && "tr" === b[0].nodeName.toLowerCase() ? b : a("<tr/>").append(a("<td/>").attr("colspan", this._colspan()).append(b))).addClass(this.c.className).addClass(g).addClass("dtrg-level-" + c)
        }
    }); h.defaults = { className: "dtrg-group", dataSrc: 0, emptyDataGroup: "No group", enable: !0, endClassName: "dtrg-end", endRender: null, startClassName: "dtrg-start", startRender: function (b, a) { return a } }; h.version = "1.1.1"; a.fn.dataTable.RowGroup = h; a.fn.DataTable.RowGroup =
        h; f.Api.register("rowGroup()", function () { return this }); f.Api.register("rowGroup().disable()", function () { return this.iterator("table", function (a) { a.rowGroup && a.rowGroup.enable(!1) }) }); f.Api.register("rowGroup().enable()", function (a) { return this.iterator("table", function (b) { b.rowGroup && b.rowGroup.enable(a === e ? !0 : a) }) }); f.Api.register("rowGroup().dataSrc()", function (a) { return a === e ? this.context[0].rowGroup.dataSrc() : this.iterator("table", function (b) { b.rowGroup && b.rowGroup.dataSrc(a) }) }); a(c).on("preInit.dt.dtrg",
            function (b, c, d) { "dt" === b.namespace && (b = c.oInit.rowGroup, d = f.defaults.rowGroup, b || d) && (d = a.extend({}, d, b), !1 !== b && new h(c, d)) }); return h
});