const urls = {
    getRoomSpaceData: 'ISP/GetRoomSpaceData',
    getRacksData: 'ISP/GetRacksData', //getRacksData: 'ISP/GetRacksData',
    getRackLibrary: 'ISP/GetRackLibrary',
    getModelType: 'ISP/GetModelType',
    deleteRack: 'ISP/DeleteRack',
    getRoomDetail: 'ISP/GetRoomParentDetails',
    getISPEntityInformationDetail: 'ISP/GetISPEntityInformationDetail',
    getWorkspaceData: 'ISP/GetEquipments',
    getRackChildren: 'ISP/GetRackChildren',
    getVendorList: '../ItemTemplate/GetVendorList',
    getSubVendorList: '../ItemTemplate/GetCatSubcatData',
    deleteEquipment: 'ISP/DeleteEquipment',
    saveEquipmentPosition: 'ISP/SaveEquipmentPosition',
    saveRackPosition: 'ISP/SaveRackPosition',
    getUnmappedFMS: 'ISP/GetUnmappedFMS',
    getMiddlewareType: 'ISP/GetMiddlewareModelType',
    getPortMultipleConnections: 'ISP/MultipleConnection',
    getConnectedPorts: 'ISP/getConnectionList',
    getOutConnectedPorts: 'splicing/getOutConnectedPorts'
};

//const doorSvgContent = {
//    ER: '<path d="M50,50 v-50 a50,50 0 0,0 -50,50z M0,49h50M0,48h50M0,47h50" fill="#bcbcbc" stroke="gray" stroke-width="1"/>',
//    EM: '<path d="M50,50 v-50 a50,50 0 0,0 -50,50 z M0,49h50M0,48h50M0,47h50" fill="#bcbcbc" stroke="gray" stroke-width="1"/>',
//    EL: '<path d="M50,0 h-50 a50,50 0 0,0 50,50 z M0,1h50M0,2h50M0,3h50" fill="#bcbcbc" stroke="gray" stroke-width="1"/>',
//    WR: '<path d="M0,0 v50 a50,50 0 0,0 50,-50 z M0,1h50M0,2h50M0,3h50" fill="#bcbcbc" stroke="gray" stroke-width="1"/>',
//    WM: '<path d="M0,0 v50 a50,50 0 0,0 50,-50 z M0,49h50M0,48h50M0,47h50" fill="#bcbcbc" stroke="gray" stroke-width="1"/>',
//    WL: '<path d="M0,50 h50 a50,50 0 0,0 -50,-50 z M0,49h50M0,48h50M0,47h50" fill="#bcbcbc" stroke="gray" stroke-width="1"/>',
//    NR: '<path d="M50,0 h-50 a50,50 0 0,0 50,50 z M47,0v50M48,0v50M49,0v50 " fill="#bcbcbc" stroke="gray" stroke-width="1"/>',
//    NM: '<path d="M50,0 h-50 a50,50 0 0,0 50,50 z M47,0v50M48,0v50M49,0v50" fill="#bcbcbc" stroke="gray" stroke-width="1"/>',
//    NL: '<path d="M0,0 v50 a50,50 0 0,0 50,-50 z M1,0v50M2,0v50M3,0v50" fill="#bcbcbc" stroke="gray" stroke-width="1"/>',
//    SR: '<path d="M0,50 h50 a50,50 0 0,0 -50,-50 z M1,0v50M2,0v50M3,0v50" fill="#bcbcbc" stroke="gray" stroke-width="1"/>',
//    SM: '<path d="M0,50 h50 a50,50 0 0,0 -50,-50 z M1,0v50M2,0v50M3,0v50" fill="#bcbcbc" stroke="gray" stroke-width="1"/>',
//    SL: '<path d="M50,50 v-50 a50,50 0 0,0 -50,50 z M47,0v50M48,0v50M49,0v50" fill="#bcbcbc" stroke="gray" stroke-width="1"/>',
//};
const doorType = {
    none: 'none',
    flushed: 'Flushed Door',
    slide: 'Sliding Door',
    shutter: 'Roller Shutter Door'
}
const flushedDoorSvgContent = {
    ER: '<path d="M2.47159e-06 51L0 2.01072e-06L51 0L51 51L2.47159e-06 51Z" fill="#E4E4E4"/><path fill-rule="evenodd" clip-rule="evenodd" d="M5 3L5 7C5 30.9809 25.0567 50.481 50 50.9898V51H51V0L50 4.37112e-08V3L5 3ZM50 49.9896V7L6.04545 7C6.04546 30.4286 25.6341 49.4809 50 49.9896Z" fill="#707070"/>',
    EM: '<path d="M2.47159e-06 51L0 2.01072e-06L51 0L51 51L2.47159e-06 51Z" fill="#E4E4E4"/><path fill-rule="evenodd" clip-rule="evenodd" d="M5 3L5 7C5 30.9809 25.0567 50.481 50 50.9898V51H51V0L50 4.37112e-08V3L5 3ZM50 49.9896V7L6.04545 7C6.04546 30.4286 25.6341 49.4809 50 49.9896Z" fill="#707070"/>',
    EL: '<path d="M2.47159e-06 0L0 51L51 51L51 2.01072e-06L2.47159e-06 0Z" fill="#E4E4E4"/><path fill-rule="evenodd" clip-rule="evenodd" d="M5 47.9999L5 44C5 44 5 44 5 44C5.00003 20.0191 25.0567 0.519047 50 0.0101912V0L51 4.37112e-08V51H50V47.9999L5 47.9999ZM50 1.01043V43.9999L6.04545 43.9999C6.04549 20.5714 25.6341 1.5191 50 1.01043Z" fill="#707070"/>',
    WR: '<path d="M51 2.01072e-06L51 51L0 51L2.47159e-06 0L51 2.01072e-06Z" fill="#E4E4E4"/><path fill-rule="evenodd" clip-rule="evenodd" d="M46 48V44C46 20.0191 25.9433 0.519049 1 0.0101913V4.37112e-08L6.04398e-06 0L0 51H0.999996L0.999996 48L46 48ZM1 1.01043L0.999996 44L44.9545 44C44.9545 20.5714 25.3659 1.5191 1 1.01043Z" fill="#707070"/>',
    WM: '<path d="M51 2.01072e-06L51 51L0 51L2.47159e-06 0L51 2.01072e-06Z" fill="#E4E4E4"/><path fill-rule="evenodd" clip-rule="evenodd" d="M46 48V44C46 20.0191 25.9433 0.519049 1 0.0101913V4.37112e-08L6.04398e-06 0L0 51H0.999996L0.999996 48L46 48ZM1 1.01043L0.999996 44L44.9545 44C44.9545 20.5714 25.3659 1.5191 1 1.01043Z" fill="#707070"/>',
    WL: '<path d="M51 51L51 0L0 2.01072e-06L2.47159e-06 51L51 51Z" fill="#E4E4E4"/><path fill-rule="evenodd" clip-rule="evenodd" d="M46 3.00005V6.99999C46 6.99998 46 7.00001 46 6.99999C46 30.9809 25.9433 50.4809 1 50.9898V51H6.04398e-06L0 4.37112e-08L0.999996 0L0.999996 3.00005L46 3.00005ZM1 49.9896L0.999996 7.00005L44.9545 7.00005C44.9545 30.4286 25.3659 49.4809 1 49.9896Z" fill="#707070"/>',
    NR: '<path d="M51 51L4.02145e-06 51L0 4.94319e-06L51 0L51 51Z" fill="#E4E4E4"/><path fill-rule="evenodd" clip-rule="evenodd" d="M3 46H7C30.9809 46 50.481 25.9433 50.9898 0.999996H51V0L0 6.43864e-07L8.74224e-08 0.999997L3 0.999997L3 46ZM49.9896 0.999996H7L7 44.9545C30.4286 44.9545 49.4809 25.3659 49.9896 0.999996Z" fill="#707070"/>',
    NM: '<path d="M51 51L4.02145e-06 51L0 4.94319e-06L51 0L51 51Z" fill="#E4E4E4"/><path fill-rule="evenodd" clip-rule="evenodd" d="M3 46H7C30.9809 46 50.481 25.9433 50.9898 0.999996H51V0L0 6.43864e-07L8.74224e-08 0.999997L3 0.999997L3 46ZM49.9896 0.999996H7L7 44.9545C30.4286 44.9545 49.4809 25.3659 49.9896 0.999996Z" fill="#707070"/>',
    NL: '<path d="M0 51H51V0H0V51Z" fill="#E4E4E4"/><path fill-rule="evenodd" clip-rule="evenodd" d="M47.9999 46H44C44 46 44 46 44 46C20.0191 46 0.519048 25.9433 0.0101912 1H0V3.8147e-06L51 0V0.999996H47.9999V46ZM1.01043 1L43.9999 0.999996V44.9545C20.5714 44.9545 1.5191 25.3659 1.01043 1Z" fill="#707070"/>',
    SR: '<rect width="51" height="51" fill="#E4E4E4"/><path fill-rule="evenodd" clip-rule="evenodd" d="M48 5H44C20.0191 5 0.519048 25.0567 0.0101912 50H0V51L51 51V50H48V5ZM1.01043 50L44 50V6.04545C20.5714 6.04545 1.5191 25.6341 1.01043 50Z" fill="#707070"/>',
    SM: '<rect width="51" height="51" fill="#E4E4E4"/><path fill-rule="evenodd" clip-rule="evenodd" d="M48 5H44C20.0191 5 0.519048 25.0567 0.0101912 50H0V51L51 51V50H48V5ZM1.01043 50L44 50V6.04545C20.5714 6.04545 1.5191 25.6341 1.01043 50Z" fill="#707070"/>',
    SL: '<path d="M51 0L0 4.94319e-06L4.02145e-06 51L51 51L51 0Z" fill="#E4E4E4"/><path fill-rule="evenodd" clip-rule="evenodd" d="M3.00005 5L6.99999 5C30.9809 5.00003 50.4809 25.0567 50.9898 50H51V51L8.74224e-08 51L0 50H3.00005L3.00005 5ZM49.9896 50L7.00005 50L7.00005 6.04545C30.4286 6.04548 49.4809 25.6341 49.9896 50Z" fill="#707070"/>',
};
const slideDoorSvgContent = {
    ER: '<path d="M9 3.93402e-07L9 85H0L3.71547e-06 0L9 3.93402e-07Z" fill="#E4E4E4"/><path d="M8 46L5 46L5 0L8 1.78834e-07L8 46Z" fill="#C7C7C7"/><path d="M5 85L2 85L2 39L5 39L5 85Z" fill="#C7C7C7"/><path fill-rule="evenodd" clip-rule="evenodd" d="M5 0L8 1.31134e-07L8 46H5L5 0ZM6 41H7L7 5H6L6 41Z" fill="#707070"/><path fill-rule="evenodd" clip-rule="evenodd" d="M2 39H5L5 85H2L2 39ZM3 80H4L4 44H3L3 80Z" fill="#707070"/>',
    EM: '<path d="M9 3.93402e-07L9 85H0L3.71547e-06 0L9 3.93402e-07Z" fill="#E4E4E4"/><path d="M8 46L5 46L5 0L8 1.78834e-07L8 46Z" fill="#C7C7C7"/><path d="M5 85L2 85L2 39L5 39L5 85Z" fill="#C7C7C7"/><path fill-rule="evenodd" clip-rule="evenodd" d="M5 0L8 1.31134e-07L8 46H5L5 0ZM6 41H7L7 5H6L6 41Z" fill="#707070"/><path fill-rule="evenodd" clip-rule="evenodd" d="M2 39H5L5 85H2L2 39ZM3 80H4L4 44H3L3 80Z" fill="#707070"/>',
    EL: '<path d="M9 3.93402e-07L9 85H0L3.71547e-06 0L9 3.93402e-07Z" fill="#E4E4E4"/><path d="M8 46L5 46L5 0L8 1.78834e-07L8 46Z" fill="#C7C7C7"/><path d="M5 85L2 85L2 39L5 39L5 85Z" fill="#C7C7C7"/><path fill-rule="evenodd" clip-rule="evenodd" d="M5 0L8 1.31134e-07L8 46H5L5 0ZM6 41H7L7 5H6L6 41Z" fill="#707070"/><path fill-rule="evenodd" clip-rule="evenodd" d="M2 39H5L5 85H2L2 39ZM3 80H4L4 44H3L3 80Z" fill="#707070"/>',
    WR: '<path d="M9 3.93402e-07L9 85H0L3.71547e-06 0L9 3.93402e-07Z" fill="#E4E4E4"/><path d="M8 46L5 46L5 0L8 1.78834e-07L8 46Z" fill="#C7C7C7"/><path d="M5 85L2 85L2 39L5 39L5 85Z" fill="#C7C7C7"/><path fill-rule="evenodd" clip-rule="evenodd" d="M5 0L8 1.31134e-07L8 46H5L5 0ZM6 41H7L7 5H6L6 41Z" fill="#707070"/><path fill-rule="evenodd" clip-rule="evenodd" d="M2 39H5L5 85H2L2 39ZM3 80H4L4 44H3L3 80Z" fill="#707070"/>',
    WM: '<path d="M9 3.93402e-07L9 85H0L3.71547e-06 0L9 3.93402e-07Z" fill="#E4E4E4"/><path d="M8 46L5 46L5 0L8 1.78834e-07L8 46Z" fill="#C7C7C7"/><path d="M5 85L2 85L2 39L5 39L5 85Z" fill="#C7C7C7"/><path fill-rule="evenodd" clip-rule="evenodd" d="M5 0L8 1.31134e-07L8 46H5L5 0ZM6 41H7L7 5H6L6 41Z" fill="#707070"/><path fill-rule="evenodd" clip-rule="evenodd" d="M2 39H5L5 85H2L2 39ZM3 80H4L4 44H3L3 80Z" fill="#707070"/>',
    WL: '<path d="M9 3.93402e-07L9 85H0L3.71547e-06 0L9 3.93402e-07Z" fill="#E4E4E4"/><path d="M8 46L5 46L5 0L8 1.78834e-07L8 46Z" fill="#C7C7C7"/><path d="M5 85L2 85L2 39L5 39L5 85Z" fill="#C7C7C7"/><path fill-rule="evenodd" clip-rule="evenodd" d="M5 0L8 1.31134e-07L8 46H5L5 0ZM6 41H7L7 5H6L6 41Z" fill="#707070"/><path fill-rule="evenodd" clip-rule="evenodd" d="M2 39H5L5 85H2L2 39ZM3 80H4L4 44H3L3 80Z" fill="#707070"/>',
    NR: '<rect width="85" height="9" fill="#E4E4E4"/><path d="M46 1L46 4L0 4L4.76995e-08 1L46 1Z" fill="#C7C7C7"/><path d="M85 4L85 7L39 7L39 4L85 4Z" fill="#C7C7C7"/><path fill-rule="evenodd" clip-rule="evenodd" d="M0 4V1H46V4L0 4ZM41 3V2L5 2V3L41 3Z" fill="#707070"/><path fill-rule="evenodd" clip-rule="evenodd" d="M39 7V4H85V7L39 7ZM80 6V5L44 5V6L80 6Z" fill="#707070"/>',
    NM: '<rect width="85" height="9" fill="#E4E4E4"/><path d="M46 1L46 4L0 4L4.76995e-08 1L46 1Z" fill="#C7C7C7"/><path d="M85 4L85 7L39 7L39 4L85 4Z" fill="#C7C7C7"/><path fill-rule="evenodd" clip-rule="evenodd" d="M0 4V1H46V4L0 4ZM41 3V2L5 2V3L41 3Z" fill="#707070"/><path fill-rule="evenodd" clip-rule="evenodd" d="M39 7V4H85V7L39 7ZM80 6V5L44 5V6L80 6Z" fill="#707070"/>',
    NL: '<rect width="85" height="9" fill="#E4E4E4"/><path d="M46 1L46 4L0 4L4.76995e-08 1L46 1Z" fill="#C7C7C7"/><path d="M85 4L85 7L39 7L39 4L85 4Z" fill="#C7C7C7"/><path fill-rule="evenodd" clip-rule="evenodd" d="M0 4V1H46V4L0 4ZM41 3V2L5 2V3L41 3Z" fill="#707070"/><path fill-rule="evenodd" clip-rule="evenodd" d="M39 7V4H85V7L39 7ZM80 6V5L44 5V6L80 6Z" fill="#707070"/>',
    SR: '<rect width="85" height="9" fill="#E4E4E4"/><path d="M46 1L46 4L0 4L4.76995e-08 1L46 1Z" fill="#C7C7C7"/><path d="M85 4L85 7L39 7L39 4L85 4Z" fill="#C7C7C7"/><path fill-rule="evenodd" clip-rule="evenodd" d="M0 4V1H46V4L0 4ZM41 3V2L5 2V3L41 3Z" fill="#707070"/><path fill-rule="evenodd" clip-rule="evenodd" d="M39 7V4H85V7L39 7ZM80 6V5L44 5V6L80 6Z" fill="#707070"/>',
    SM: '<rect width="85" height="9" fill="#E4E4E4"/><path d="M46 1L46 4L0 4L4.76995e-08 1L46 1Z" fill="#C7C7C7"/><path d="M85 4L85 7L39 7L39 4L85 4Z" fill="#C7C7C7"/><path fill-rule="evenodd" clip-rule="evenodd" d="M0 4V1H46V4L0 4ZM41 3V2L5 2V3L41 3Z" fill="#707070"/><path fill-rule="evenodd" clip-rule="evenodd" d="M39 7V4H85V7L39 7ZM80 6V5L44 5V6L80 6Z" fill="#707070"/>',
    SL: '<rect width="85" height="9" fill="#E4E4E4"/><path d="M46 1L46 4L0 4L4.76995e-08 1L46 1Z" fill="#C7C7C7"/><path d="M85 4L85 7L39 7L39 4L85 4Z" fill="#C7C7C7"/><path fill-rule="evenodd" clip-rule="evenodd" d="M0 4V1H46V4L0 4ZM41 3V2L5 2V3L41 3Z" fill="#707070"/><path fill-rule="evenodd" clip-rule="evenodd" d="M39 7V4H85V7L39 7ZM80 6V5L44 5V6L80 6Z" fill="#707070"/>',
};
const shutterDoorSvgContent = {
    ER: '<path d="M16 3.70261e-07L16 80H0L6.60527e-06 0L16 3.70261e-07Z" fill="#E4E4E4"/>',
    EM: '<path d="M16 3.70261e-07L16 80H0L6.60527e-06 0L16 3.70261e-07Z" fill="#E4E4E4"/>',
    EL: '<path d="M16 3.70261e-07L16 80H0L6.60527e-06 0L16 3.70261e-07Z" fill="#E4E4E4"/>',
    WR: '<path d="M16 3.70261e-07L16 80H0L6.60527e-06 0L16 3.70261e-07Z" fill="#E4E4E4"/>',
    WM: '<path d="M16 3.70261e-07L16 80H0L6.60527e-06 0L16 3.70261e-07Z" fill="#E4E4E4"/>',
    WL: '<path d="M16 3.70261e-07L16 80H0L6.60527e-06 0L16 3.70261e-07Z" fill="#E4E4E4"/>',
    NR: '<path d="M80 16L0 16L6.99382e-07 0L80 1.01022e-05V16Z" fill="#E4E4E4"/>',
    NM: '<path d="M80 16L0 16L6.99382e-07 0L80 1.01022e-05V16Z" fill="#E4E4E4"/>',
    NL: '<path d="M80 16L0 16L6.99382e-07 0L80 1.01022e-05V16Z" fill="#E4E4E4"/>',
    SR: '<path d="M80 16L0 16L6.99382e-07 0L80 1.01022e-05V16Z" fill="#E4E4E4"/>',
    SM: '<path d="M80 16L0 16L6.99382e-07 0L80 1.01022e-05V16Z" fill="#E4E4E4"/>',
    SL: '<path d="M80 16L0 16L6.99382e-07 0L80 1.01022e-05V16Z" fill="#E4E4E4"/>',
};
const rackGlobalData = {
    svgContent: '<path d="M40 0H0V24H40V0Z" fill="[COLOR]"/><path d="M4 3H6V21H4V3Z" fill="white"/><path d="M12 3H10V21H12V3Z" fill="white"/><path d="M18 3H16V21H18V3Z" fill="white"/><path d="M24 3H22V21H24V3Z" fill="white"/><path d="M30 3H28V21H30V3Z" fill="white"/><path d="M36 3H34V21H36V3Z" fill="white"/>'
};
const equipmentGlobalData = {
    svgContent: '<path fill="[COLOR]" fill-rule="evenodd" clip-rule="evenodd" d="M0 0H23V8H0V0ZM21.7894 1.14286H16.9473V3.42857H21.7894V1.14286ZM21.7894 4.57146H16.9473V6.85718H21.7894V4.57146ZM1.21047 2.28571H3.63152V3.42857H1.21047V2.28571ZM1.21048 4.5715H3.63154V5.71436H1.21048V4.5715ZM7.26319 2.28578H4.84214V3.42864H7.26319V2.28578ZM4.84214 4.5715H7.26319V5.71436H4.84214V4.5715ZM10.8947 2.28578H8.47366V3.42864H10.8947V2.28578ZM8.47366 4.5715H10.8947V5.71436H8.47366V4.5715ZM14.5264 2.28578H12.1053V3.42864H14.5264V2.28578ZM12.1053 4.5715H14.5264V5.71436H12.1053V4.5715Z" fill="#4C6788"/>'
};
const networkRackColor = {
    A: '#BDBDBD',
    P: '#57C15E',
    D: '#F04CF0',
    none: '#5897FB'
};
const equipmentStatsSvg = {
    topLeft: '<svg width="16" height="16" viewBox="0 0 16 16" fill="none" xmlns="http://www.w3.org/2000/svg"><path d="M16 0L0 0V16H2.66667L16 2.66667V0Z" fill="[COLOR]"/><path d="M2.66663 2.66663L16 2.66663L2.66663 16V2.66663Z" fill="[BCOLOR]"/></svg>',
    topRight: '<svg width="16" height="16" viewBox="0 0 16 16" fill="none" xmlns="http://www.w3.org/2000/svg"><path d="M16 16L16 6.99382e-07L1.16564e-07 0L0 2.66667L13.3333 16H16Z" fill="[COLOR]"/><path d="M13.3333 2.66666V16L0 2.66666L13.3333 2.66666Z" fill="[BCOLOR]"/></svg>',
    bottomLeft: '<svg width="16" height="16" viewBox="0 0 16 16" fill="none" xmlns="http://www.w3.org/2000/svg"><path d="M2.09815e-06 0L0 16L16 16V13.3333L2.66667 3.49691e-07L2.09815e-06 0Z" fill="[COLOR]"/><path d="M2.66663 13.3333L2.66663 0L16 13.3333L2.66663 13.3333Z" fill="[BCOLOR]"/></svg>',
    bottomRight: '<svg width="16" height="16" viewBox="0 0 16 16" fill="none" xmlns="http://www.w3.org/2000/svg"><path d="M0 16L16 16L16 2.33127e-07L13.3333 0L2.33127e-07 13.3333L0 16Z" fill="[COLOR]"/><path d="M13.3333 13.3333L0 13.3333L13.3333 -1.52588e-05L13.3333 13.3333Z" fill="[BCOLOR]"/></svg>'
};
const networkEqColor = {
    A: '#9C9B9B',
    P: '#3A8A40',
    D: '#D952D9'
};
const networkPortColor = {
    V: '#808080',
    C: '#00BA6C',
    R: '#566FEE',
    F: '#FF5454'
};
function invertColor(color) {
    color = color.substring(1);
    return '#' + (Number(`0x1${color}`) ^ 0xFFFFFF).toString(16).substr(1).toUpperCase();
}

/*DO NOT DELETE: Render selector input value sample
                {
                    data: res.result,
                    element: $(DE.selectVender),
                    value: 'value',
                    text: 'key',
                    defaultText: 'Select Vendor',
                    dataKey: [{ key: 'item_template_id', name: 'data-item-id' }],
                    disabled: false
                }
*/
this.renderSelector = function (d) {

    let option = "";
    if (d.defaultText)
        //option = "<option value disabled selected hidden>" + d.defaultText + "</option>";
        option = "<option value  selected >" + d.defaultText + "</option>";
    if (d && d.data) {
        let len = d.data.length;
        let keys = "";
        if (len > 0) {
            for (let i = 0; i < len; i++) {
                keys = "";
                if (d.dataKey && d.dataKey.length) {
                    for (let j = 0; j < d.dataKey.length; j++) {
                        keys += " " + d.dataKey[j].name + "='" + d.data[i][d.dataKey[j].key] + "'";
                    }
                }
                option += "<option value='" + d.data[i][d.value] + "' " + keys + " >" + d.data[i][d.text] + "</option>";
            }
        }
    }
    d.element.attr('disabled', d.disabled);
    d.element.html(option);
}
this.ddlChangeRemoveCss = function (selectorId, emType) {

    //var emID = $("#" + selectorId + "_chosen");
    //if (emID.children('a').text().trim() != "Select " + emType) {
    //    emID.removeClass('input-validation-error').next('span').removeClass('field-validation-error').addClass('field-validation-valid').html('');
    //}
    //else {
    //    emID.addClass('input-validation-error');
    //}
}

var roomManager = (function () {
    'use strict';
    var DE = {};
    var _$workArea, _$libArea, _$libRoomViewArea;
    var _workSpaceActions = container.action;
    var _workAreaData;
    var _libraryData = [];
    var _libraryAllData = [];
    var _nodeTree = [];
    var _selectedModel = {};
    var _hierarchyRules = [];
    var _selectMany = [];
    var _rackLibraryData = [];
    var _equipmentTypeData = [];
    var _isRoomView = true;
    var _roomAllData = [];
    var _rackAllData = [];
    var _rackIndex = 0;
    var _oldPosition = { x: 0, y: 0 };
    var _isPositionChanged = false;
    var _connectionChecked = true;
    var _unmappedRepsonse = [];
    var current_parent = { entityType: '', systemId: 0, networkId: '', structure_id: 0 };
    var _scaleFactor = {
        gridCellSize: DE.gridCellSize,
        grid_scale: DE.grid_scale,
        displayScale: DE.displayScale
    };
    var _keyPressed = {};
    var _connectionView = false;

    var action = {
        addModel: function (e) {
            _workAreaData = _workSpaceActions.getWorkArea();
            let current = _workSpaceActions.add();

            current.id = _workSpaceActions.getMaxId() + 1;
            current.position = { x: 0, y: 0 };

            current.height = DE.defaultHeight;
            current.width = DE.defaultWidth;

            current.color = DE.color_code;
            current.stroke = DE.stroke;
            current.model_type = e.model_type;
            if (e && e.id && !isNaN(e.id)) {
                current.color = e.color;
                current.img_id = isNaN(e.img_id) ? '' : e.img_id;
                current.lib_id = e.id;
                current.stroke = e.stroke;
                current.image_data = e.content;
                current.height = e.height || DE.defaultHeight;
                current.width = e.width || DE.defaultWidth;
                current.color = e.color || DE.color_code;
                current.stroke = e.stroke || DE.stroke;
                current.position = {
                    x: e.position && e.position.x || 0,
                    y: e.position && e.position.y || 0
                };
                current.border_width = e.border_width || 0;
                current.name = e.name || '';
                current.no_of_units = e.no_of_units;
                current.depth = e.depth || DE.defaultDepth;

                current.db_height = e.db_height;
                current.db_width = e.db_width;
                current.db_depth = e.db_depth;
                current.db_border_width = e.db_border_width;
                current.border_color = e.border_color;
            }
            if (e && e.model_id) {
                current.model_id = e.model_id;
                current.model_type_id = e.model_type_id;
                
                if (_workAreaData.length <= 1) {
                    current.height = DE.defaultSizes[current.model_id - 1].height;
                    current.width = DE.defaultSizes[current.model_id - 1].width;
                    current.border_width = DE.defaultSizes[current.model_id - 1].border_width;

                }
            }
            if (_workAreaData.length <= 1) {
                current.is_static = true;//_workAreaData.length == 1;
                render.setInCenter(current);
                //render.setDimension({ modelHeight: current.height, modelWidth: current.width, modelDepth: current.depth });
            }
            else {
                //if (_libraryAllData.length) {
                current.parent = _workAreaData[0].id;
                let param = {
                    parent_model_id: _selectedModel.model_id,
                    parent_model_type_id: _selectedModel.model_type_id || null,
                    child_model_id: current.model_id,
                    child_model_type_id: isNaN(current.model_type_id) ? null : current.model_type_id
                }
                if (validate.modelDrop(param)) {
                    //if (!_selectedModel.id)
                    //    current.parent = _workAreaData[0].id;
                    //else
                    //    current.parent = _selectedModel.id;
                    if (_workAreaData[0].border_width) {
                        current.position.x += _workAreaData[0].border_width;
                        current.position.y += _workAreaData[0].border_width;
                    }
                }

                if (!e.isBulk) {
                    //render.setInCenter(current, _workSpaceActions.getAbsoluteRect(current.parent));
                    render.setInView(current, _workSpaceActions.getAbsoluteRect(current.parent));
                }

            }
            //}

            $(DE.modelBorderWidth).val(current.border_width);
            return current;

        },

        initCreate: function (e) {
            console.log("Page Up and Running");
            load.workArea();
            //Validate model selection here
            //if (validate.initCreate()) {
            //    return false;
            //}

            //add here left panel action and model operations
            render.modelOperation();
            render.setWorkSpaceSize(150+_workAreaData[0].height + 50, Math.abs(_workAreaData[0].position.x) + _workAreaData[0].width + 50);

        },

        selectModel: function (d, isInit) {
            isInit = isInit || false;
            if (isInit) {
                _selectedModel = d;
                return;
            }
            if (_workSpaceActions.newModelCount() == 0 || _selectedModel && _selectedModel.id > 0 && _selectedModel.db_id > 0) {
                _selectedModel.is_static = true;
                _selectedModel = d;
            }
            if (_workSpaceActions.newModelCount() > 0) {
                _selectedModel = d;
                //alert("Please save rack first", 'warning');
            }
            //render.libListByValidation();
        },
        saveElementInfo: function (e, entityType, layerDetails, templateId, d) {
            if (validate.collision(_selectedModel.id, _selectedModel.position)) {
                let name = generate.currentName();

                alert($.validator.format(MultilingualKey.SI_OSP_GBL_JQ_GBL_072, name), "Warning");//should not overlapped with other entity!
                return false;
            }
            let param = generate.inBoundaryPos(_selectedModel.parent, _selectedModel, _selectedModel.position);
            if (!validate.inBoundary(_selectedModel.parent, param)) {
                let name = generate.currentName();

                alert($.validator.format(MultilingualKey.SI_OSP_GBL_JQ_GBL_072, name), "Warning");//should not overlapped with other entity!
                return false;
            }
            let pSystemId = 0;
            let pEntityType = null;
            let pNetworkId = null;
            let fmsNetworkId = null;
            let structureId = 0;

            if (validate.isRoomView()) {
                pSystemId = current_parent.systemId;
                pEntityType = current_parent.entityType;
                pNetworkId = current_parent.networkId;

            }
            else {


                pSystemId = current_parent.systemId;
                pEntityType = current_parent.entityType;
                pNetworkId = current_parent.networkId;
                if (validate.isFMSMapping()) {
                    fmsNetworkId = $(DE.selectExistedFms + " option:selected").data('networkId');
                }

            }
            structureId = current_parent.structure_id;

            if (pSystemId == 0) {
                alert(MultilingualKey.SI_OSP_GBL_JQ_GBL_073);//Please save rack first!
                return;
            }
            if (generate.toBoolean(layerDetails.is_direct_save)) {
                let data = {
                    networkIdType: layerDetails.network_id_type,
                    ModelInfo: {
                        elementType: layerDetails.layerName,
                        structureid: structureId,
                        templateId: templateId,
                    },

                    systemId: _selectedModel.db_id,
                    pSystemId: pSystemId,
                    pEntityType: pEntityType,
                    isDirectSave: true,
                    pNetworkId: pNetworkId,
                    specificationId: _selectedModel.lib_id,
                    pos_X: generate.pixelToMM(_selectedModel.position.x),
                    pos_Y: generate.pixelToMM(_selectedModel.position.y),
                    model_info_id: _selectedModel.lib_id,
                    rack_id:  _selectedModel.model_type != undefined && _selectedModel.model_type.toUpperCase() == 'EQUIPMENT'? 0: _rackIndex,
                    fms_network_id: fmsNetworkId
                };
                 
                ajaxReq(layerDetails.save_entity_url, data, false, function (response) {
                    if (response.status == 'OK') {
                        _selectedModel.is_static = true;
                        render.modelFocus();
                        alert(response.message, 'success', 'success');
                    } else { alert(response.message, "warning"); }
                }, true, false);
            }
            else {
                var pageUrl = layerDetails.layer_form_url;
                var modalClass = 'modal-lg';
                popup.LoadModalDialog('PARENT', pageUrl, {
                    networkIdType: layerDetails.network_id_type,
                    ModelInfo: {
                        elementType: layerDetails.layerName,
                        structureid: structureId,
                        templateId: templateId,
                    },

                    systemId: _selectedModel.db_id,
                    pSystemId: pSystemId,
                    pEntityType: pEntityType,
                    pNetworkId: pNetworkId,
                    specificationId: _selectedModel.lib_id,
                    pos_X: generate.pixelToMM(_selectedModel.position.x),
                    pos_Y: generate.pixelToMM(_selectedModel.position.y),
                    model_info_id: _selectedModel.lib_id,
                    rack_id:  _selectedModel.model_type != undefined && _selectedModel.model_type.toUpperCase() == 'EQUIPMENT'? 0: _rackIndex,
                    fms_network_id: fmsNetworkId
                }, generate.currentName(), modalClass);

            }

        },

        setNewId: function (id, name) {
            _selectedModel.db_id = id;
            if (_selectedModel.border_width)
                _selectedModel.border_width = generate.pixelToMeter(_selectedModel.border_width);
            if (name)
                _selectedModel.name = name;
            render.lnkRackView(validate.isRoomView() && _workAreaData.length > 1);
            render.lnkConnection(_workAreaData.length > 1);
            render.allModels();
            _selectedModel.is_static = true;
            if (validate.isRoomInfo()) { action.entityInformation(_selectedModel); }
            else {
                _workAreaData = _workAreaData.filter(x=>x.id == 1);
                if (!validate.isRoomView()) {
                    action.getRackChildren(_rackIndex, false);
                    if (validate.isFMSMapping()) { action.getUnmappedFMS(current_parent.systemId, current_parent.entityType); }
                }
            }
        },
        initRoom: function (res, type) {
            action.clear();
            action.selectModel({});
            current_parent.networkId = res.network_id;
            current_parent.structure_id = res.structure_id;
            render.setRoomTitle(current_parent.networkId + "(" + res.name + ")");
            //if (type.toLowerCase() == 'unit') {
            DE.defaultHeight = generate.meterToPixel(res.room_length);
            DE.defaultWidth = generate.meterToPixel(res.room_width);
            DE.defaultDoor = res.door_position;
            DE.defaultDoorWidth = res.door_width;
            DE.defaultDoorType = res.door_type;

            //Set room door 
            //render.roomDoor({ position: DE.defaultDoor, width: DE.defaultDoorWidth, type: DE.defaultDoorType });
            //}
            //else {
            //    DE.defaultHeight = generate.meterToPixel(res.length);
            //    DE.defaultWidth = generate.meterToPixel(res.width);
            //    DE.defaultDoor = null;
            //    DE.defaultDoorWidth = 0;
            //    DE.defaultDoorType = 'none';
            //}
            render.roomView(true);
            render.leftPanel(true);
            render.setWorkSpaceSize(DE.defaultHeight + 50, DE.defaultWidth + 50);
            action.initCreate();
            event.onLibFilterOption();
        },
        setRackChildren: function () {
            _workAreaData = _workSpaceActions.getWorkArea();
            //$.extend(true, _rackAllData, _workAreaData);
            if (_roomAllData.length > 1) {
                if (_selectedModel.id > 1) {
                    let rackFound = _roomAllData.filter(x=> x.db_id == _workAreaData[0].db_id)
                    rackFound[0].children = [];
                    $.extend(true, rackFound[0].children, _workAreaData);
                }
                else {
                    let rackFound = _roomAllData[1];
                    $.extend(true, rackFound.children, _workAreaData);
                }
            }
            _workAreaData = [];
            $.extend(true, _workAreaData, _roomAllData);
            _workSpaceActions.setWorkArea(_workAreaData);
        },
        setEquipmentChildren: function () {
            _workAreaData = _workSpaceActions.getWorkArea();
            //$.extend(true, _rackAllData, _workAreaData);
            if (_roomAllData.length > 1) {
                if (_selectedModel.id > 1) {
                    let rackFound = _roomAllData.filter(x=> x.model_type.toUpperCase() == 'EQUIPMENT')
                    rackFound[0].children = [];
                    $.extend(true, rackFound[0].children, _workAreaData);
                }
                else {
                    let rackFound = _roomAllData[1];
                    $.extend(true, rackFound.children, _workAreaData);
                }
            }
            _workAreaData = [];
            $.extend(true, _workAreaData, _roomAllData);
            _workSpaceActions.setWorkArea(_workAreaData);
        },
        clear: function () {
            _roomAllData = [];
            _rackAllData = [];
            _rackIndex = 0;
            _workSpaceActions.reset();
        },
        remove: function () {
            let url = "";
            let isEquipment = false;
            if (validate.isRoomView()) {
                if (_selectedModel.model_type != undefined && _selectedModel.model_type.toUpperCase() == 'EQUIPMENT') {
                    url = urls.deleteEquipment;
                    isEquipment = true;
                }else {
                    url = urls.deleteRack;
                }
        
            }else {
                url = urls.deleteEquipment;
                isEquipment = true;
            }
    
            API.call(url, { id: _selectedModel.db_id }, function (res) {
                if (isEquipment) { EquipmentEditor.action.resetOnDelete(_selectedModel.db_id); }
                if (res.status.toUpperCase() == 'OK' || _selectedModel.db_id == 0) {
                    let deletedName = generate.currentName();
                    _workSpaceActions.remove(_selectedModel.id);
                    action.selectModel({});
                    _workAreaData = _workSpaceActions.getWorkArea();
                    if (validate.isRoomView()) {
                        render.lnkRackView(_workAreaData.length > 1);
                        render.lnkConnection(_workAreaData.length > 1);
                    }
                    else {
                        if (validate.isFMSMapping()) { action.getUnmappedFMS(current_parent.systemId, current_parent.entityType); }
                    }
                    render.allModels();
                    if (_workAreaData.length == 1) {
                        $(DE.roomLeftPanel).show();
                        $(DE.entityInfoPanel).hide();
                    }

                    alert($.validator.format(MultilingualKey.SI_OSP_GBL_JQ_GBL_074, deletedName), 'success');// {0} deleted successfully!
                }
                else {
                    if (validate.isRoomView()) {
                        alert(res.message, 'Warning');
                    }
                    else { alert(res.message, 'Warning'); }
                }
            });
        },
        editRack: function () {
            _selectedModel.is_static = false;
            if (_selectedModel.is_editable) {
                _oldPosition.x = _selectedModel.position.x;
                _oldPosition.y = _selectedModel.position.y;
                _isPositionChanged = true;
            }

            render.editPosition(_isPositionChanged);
        },
        cancelRackEdit: function () {
            if (_isPositionChanged && !_selectedModel.is_static && _selectedModel.id > 0) {
                _selectedModel.is_static = true;
                _selectedModel.position.x = _oldPosition.x;
                _selectedModel.position.y = _oldPosition.y;
                //let $modelFound = d3.select("#" + DE.modelElementId + _selectedModel.id);
                //$modelFound.attr("x", generate.modelPositionX(_selectedModel)).attr("y", generate.modelPositionY(_selectedModel));

                let modelGroupFound = d3.select("#" + DE.modelGroup + _selectedModel.id);
                modelGroupFound.attr("transform", function () {
                    return 'translate(' + generate.modelPositionX(_selectedModel) + ',' + generate.modelPositionY(_selectedModel) + ') rotate(' + _selectedModel.rotation_angle + "," + _selectedModel.width / 2 + "," + _selectedModel.height / 2 + ")";
                });
                render.equipmentStatus(_selectedModel);
                _isPositionChanged = false;
                render.editPosition(_isPositionChanged);
            }
            //render.showEditSaveCancel(false);

        },
        saveRack: function (e) {
            let entityType = 'Rack';

            let layerDetails = getLayerDetail(entityType);
            if (layerDetails != null) {
                action.saveElementInfo(e, entityType, layerDetails, 0);
            }
            //_selectedModel.is_static = true;
        },
        saveEquipment: function (e) {
            let entityType = 'Equipment';

            let layerDetails = getLayerDetail(entityType);
            if (layerDetails != null) {
                action.saveElementInfo(e, entityType, layerDetails, 0);
            }
            // _selectedModel.is_static = true;
        },

        showEntityInformationDetail: function (systemID, eName, eTitle, gType, backbutton, nStatus) {
            $("#EquipmentID").val(systemID);
            render.infoProgress(DE.roomRackInfo);
            ajaxReq(urls.getISPEntityInformationDetail, { systemId: systemID, entityName: eName, entityTitle: eTitle, geomType: gType, networkStatus: nStatus }, true, function (resp) {
                
                resp = resp.replace('id="divDetail"', 'id="divDetailroom"');
                resp = resp.replace('id="divImage"', 'id="divImageroom"');
                resp = resp.replace('id="divDocument"', 'id="divDocumentroom"');
                resp = resp.replace('href="#divDetail"', 'href="#divDetailroom"');
                resp = resp.replace('href="#divImage"', 'href="#divImageroom"');
                resp = resp.replace('href="#divDocument"', 'href="#divDocumentroom"');
                resp = resp.replace('id="divRefLink"', 'id="divRefLinkroom"');
                resp = resp.replace('href="#divRefLink"', 'href="#divRefLinkroom"');
                resp = resp.replace('value="ValuesDoneWithISP"', 'value="ValuesDoneWithRoom"');


                $(DE.roomRackInfo).html(resp);
                $(DE.roomRackInfo + ' .infodiv').animate({ 'margin-left': '-450px' }, 700);
                $(DE.roomRackInfo + ' .infoBackAction').hide();
                //show hide the NoRecordFound div 
                if ($('.panel-body table tr').length == 0)
                    $(DE.roomRackInfo + ' .NoRecordFound').show();
                else
                    $(DE.roomRackInfo + ' .NoRecordFound').hide();

                layerActions.networkStatus.bind(systemID, eName, nStatus);

            }, false, false, true);
        },
        setScale: function () {
            if (validate.isRoomView()) {
                _scaleFactor.grid_scale = DE.grid_scale;
                _scaleFactor.gridCellSize = DE.gridCellSize;
                _scaleFactor.displayScale = DE.displayScale;
            }
            else {
                _scaleFactor.grid_scale = DE.eq_grid_scale;
                _scaleFactor.gridCellSize = DE.eq_gridCellSize;
                _scaleFactor.displayScale = DE.eq_displayScale;
            }
        },
        changeRackAreaScale: function (areaData) {
            areaData.width = generate.mmToPixel(generate.meterToMM(areaData.db_width));
            areaData.height = generate.mmToPixel(generate.meterToMM(areaData.db_depth));
            areaData.depth = generate.mmToPixel(generate.meterToMM(areaData.db_height));
            areaData.outer_border_width = generate.mmToPixel(areaData.db_border_width || 0);
            areaData.position.x = areaData.width / 2 + areaData.outer_border_width + 50;
            areaData.position.y = areaData.height / 2 + 50;
            areaData.color = 'url(#rackGrid)';
        },
        changeNoRackAreaScale: function (areaData, objDimension) {
            areaData.width = generate.mmToPixel(objDimension.width);
            areaData.height = generate.mmToPixel(objDimension.length);//generate.mmToPixel(generate.meterToMM(areaData.db_depth));
            areaData.depth = generate.mmToPixel(objDimension.height);
            areaData.outer_border_width = generate.mmToPixel(areaData.db_border_width || 0);
            areaData.position.x = areaData.width / 2 + areaData.outer_border_width + 50;
            areaData.position.y = areaData.height / 2 + 50;
            areaData.color = 'url(#rackGrid)';
        },
        getNoRackDimensions: function (roomAllData) {
            var rackAllData = roomAllData.filter(x=> x.model_type != undefined && x.model_type.toUpperCase() == 'EQUIPMENT')
            var totalHeight = 0;
            var maxWidth = 0;
            var totalLength = 0;
            $.each(rackAllData, function( i, e ) {
                totalHeight += e.db_depth;
                totalLength += e.db_height+10;
                if (maxWidth < e.db_width) {
                    maxWidth = e.db_width;
                }
                //if (maxlength < e.db_height) {
                //    maxlength = e.db_height;
                //}
            });
            return { height: totalHeight, width: maxWidth, length: totalLength };
        },
        getRackChildren(id, isCheck) {
            isCheck = isCheck == undefined ? true : isCheck;
            $('body #dvProgress').show();
            //console.log("parent_id : " + current_parent.system_id + "   parent_type : " + current_parent.entity_type);
            API.call(urls.getRackChildren, { rackId: id, parent_id: current_parent.systemId, parent_type: current_parent.entityType }, function (res) {
                //console.log(res, "load children");
                if (res.length > 0 && !validate.isRoomView()) {
                    let check = _workAreaData.filter(x=> x.db_id == res[0].db_id && x.model_id);
                    //if (id == 0)
                    //{
                    //    check = _workAreaData.filter(x => x.model_type != undefined && x.model_type.toUpperCase() == 'EQUIPMENT');
                    //}

                    if (!check || !check.length || !isCheck) {
                        _workAreaData = _workAreaData.concat(res.filter(x=> x.rack_id == _rackIndex));
                        _workSpaceActions.setWorkArea(_workAreaData);
                        _workAreaData = _workSpaceActions.getWorkArea();


                        render.allModels();
                        if (isCheck) {
                            //action.selectFirstEquipment();
                            render.modelFocus();
                        }
                        if (!isCheck) {
                            let tmp = _workAreaData.filter(x=> x.db_id == _selectedModel.db_id && x.model_id);

                            if (tmp.length) {
                                action.selectModel(tmp[0], true);
                                render.modelFocus(tmp[0].id);
                            }
                        }
                    }
                }
                $('body #dvProgress').hide();
            });
        },
        rackView: function () {
             
            render.infoToLeftPnl();
            render.rackTabs();
            render.resetWorkArea();
            _isRoomView = false;
            action.setScale();
            render.rackView(true);
            render.rackViewFooter(true);
            _roomAllData = [];

            _workAreaData = _workSpaceActions.getWorkArea();
            $.extend(true, _roomAllData, _workAreaData);
            _workAreaData = [];
            let rackId = 0, rackDB_id;
            if (_roomAllData.length > 1) {
                if (_selectedModel.id > 1 && _selectedModel.db_id > 0 && _selectedModel.model_type != undefined && _selectedModel.model_type.toUpperCase() != 'EQUIPMENT') {
                    _rackAllData = _roomAllData.filter(x=> x.db_id == _selectedModel.db_id);
                    if (_rackAllData[0]) {
                        generate.rackChildren(_rackAllData[0]);
                        $.extend(true, _workAreaData, _rackAllData[0].children);
                        rackId = _rackAllData[0].id;
                        rackDB_id = _rackAllData[0].db_id;
                    } else {
                        _rackAllData = _roomAllData[1];
                        generate.rackChildren(_rackAllData);
                        $.extend(true, _workAreaData, _rackAllData.children);
                        rackId = _rackAllData.id;
                        rackDB_id = _rackAllData.db_id;
                    }
                }
                else {
                    _rackAllData = _roomAllData[1];
                    generate.rackChildren(_rackAllData);
                    $.extend(true, _workAreaData, _rackAllData.children);
                    rackId = _rackAllData.id;
                    rackDB_id = _rackAllData.db_id;
                }
            }

            _workSpaceActions.setWorkArea(_workAreaData);

            render.workAreaSwitch();

            if (rackDB_id == 999999) {
                var objDimension = action.getNoRackDimensions(_roomAllData);
                action.changeNoRackAreaScale(_workAreaData[0], objDimension);
                //render.setWorkSpaceSize(objDimension.height + 150, objDimension.width );
            }
            else {
                action.changeRackAreaScale(_workAreaData[0]);
            }
          //  action.changeRackAreaScale(_workAreaData[0]);

            render.setWorkSpaceSize(Math.abs(generate.modelPositionY(_workAreaData[0])) + _workAreaData[0].height + 50, Math.abs(generate.modelPositionX(_workAreaData[0])) + _workAreaData[0].width + (_workAreaData[0].db_border_width) + 50);
            //render.setInTop(_workAreaData[0]);
            //action.selectFirstEquipment();
            //action.selectModel({});
            render.allModels();
            render.scaleText('1M(Grid Module) = ' + DE.eq_grid_scale + ' mm');
            render.activeRackTab($('#rackTab' + rackId));
            action.getRackChildren(_workAreaData[0].db_id);
            event.onSearchLibraryData();
            _rackIndex = rackDB_id;
            if (rackDB_id == 999999) {
                //if ((_rackAllData.length == 1 || _rackAllData.length == undefined)) {
                //    if (!validate.isRoomView()){
                //        $(DE.navRoomLib).hide();
                //    } else {
                //        $(DE.navRoomLib).show();
                //    }
                //} 
                $(DE.navRoomLib).hide();
                $(DE.roomLeftPanel).hide();
                $(DE.rackAreaContext).addClass("withoutRack")
            }
            else {
               $(DE.navRoomLib).show();
                $(DE.roomLeftPanel).show();
                $(DE.rackAreaContext).removeClass("withoutRack")
            }

            if (rackDB_id == 999999 && (_rackAllData.length == 1 || _rackAllData.length == undefined)) {
                $(DE.rackAreaContext).hide();
            } else {
                $(DE.rackAreaContext).show();
            }

        },
        savePosition: function (e) {
            let url = '';
            let param = {};
            if (validate.isRoomView()) {
                url = urls.saveRackPosition;
                if (_selectedModel.model_type != undefined && _selectedModel.model_type.toUpperCase() == 'EQUIPMENT') {
                    url = urls.saveEquipmentPosition;
                }
                
                param = { system_id: _selectedModel.db_id, pos_x: generate.pixelToMM(_selectedModel.position.x), pos_y: generate.pixelToMM(_selectedModel.position.y), pos_z: 0 };
            }
            else {
                url = urls.saveEquipmentPosition;
                param = { system_id: _selectedModel.db_id, pos_x: generate.pixelToMM(_selectedModel.position.x), pos_y: generate.pixelToMM(_selectedModel.position.y), pos_z: 0 };
            }
            API.call(url, param, function (response) {
                if (response.status == "OK") {
                    _selectedModel.is_static = true;
                    _isPositionChanged = false;
                    render.editPosition(_isPositionChanged);
                    alert(response.message, response.status == "OK" ? 'success' : 'warning', response.status == "OK" ? 'success' : 'information');
                }

            });

        },
        getElementImages: function () {
            render.infoProgress(DE.roomRackInfo + ' ' + DE.divImage);
            var _system_Id = $(DE.roomRackInfo + ' ' + DE.liImage).data().systemId;
            var _entity_type = $(DE.roomRackInfo + ' ' + DE.liImage).data().entityType;
            ajaxReq('ISP/getISPEntityImages', { system_Id: _system_Id, entity_type: _entity_type }, true, function (jResp) {
                jResp = jResp.replace('id="divDeleteImages"', 'id="divDeleteImagesroom"');
                $(DE.roomRackInfo + ' ' + DE.divImage).html(jResp);
                $('#OrbitHolder').orbit();
            }, false, false, true);

            setTimeout(function () {
                $(".deleteRKImgs").each(function () {
                     
                    var cliks = $(this).attr("onclick");
                    cliks = cliks.replace("isp.getElementImages", "roomManager.getElementImages");
                    $(this).attr("onclick", cliks);
                });
            }, 1000);
        },
        uploadDocumentFile: function () {
            var frmData = new FormData();
            var filesize = $('#hdnMaxFileUploadSizeLimit').val();

            var Sizeinbytes = filesize * 1024;
            if ($(DE.roomRackInfo + ' ' + DE.uploadDocument).get(0).files[0].size > Sizeinbytes) {
                //File size is too large. Maximum file size allowed is

                alert(getMultilingualStringValue($.validator.format(MultilingualKey.SI_OSP_GBL_JQ_FRM_109, (filesize / 1024).toFixed(2))));
            }
            else {
                var uploadedfile = $(DE.roomRackInfo + ' ' + DE.uploadDocument).get(0).files[0];
                if (!action.validateDocumentFileType()) { return false; }
                frmData.append(uploadedfile.name, uploadedfile);
                frmData.append('system_Id', $(DE.roomRackInfo + ' ' + DE.liDocument).data().systemId);
                frmData.append('entity_type', $(DE.roomRackInfo + ' ' + DE.liDocument).data().entityType);
                ajaxReqforFileUpload('Main/UploadDocument', frmData, true, function (resp) {
                    if (resp.status == "OK") {
                        action.getAttachmentFiles();
                        alert(resp.message, 'success', 'success');
                        if ($(DE.roomRackInfo + ' ' + DE.uploadDocument)[0] != undefined)
                            $(DE.roomRackInfo + ' ' + DE.uploadDocument)[0].value = '';
                    }
                    else {
                        alert(resp.message, 'warning');
                    }

                }, true);
            }
        },
        uploadImageFile: function () {
            //Get File from uploader and prepare form data to post.
            var frmData = new FormData();
            var filesize = $('#hdnMaxFileUploadSizeLimit').val();
            var Sizeinbytes = filesize * 1024;
            let file = $(DE.roomRackInfo + ' ' + DE.uploadImage).get(0).files[0];
            if (file && file.size > Sizeinbytes) {

                var validFileSizeinMB = filesize % 1024 == 0 ? parseInt(filesize / 1024) : (filesize / 1024).toFixed(2);
                //Image size is too large. Maximum image size allowed is
                alert(getMultilingualStringValue($.validator.format(MultilingualKey.SI_OSP_GBL_JQ_GBL_112, validFileSizeinMB)));
            }
            else {
                var uploadedfile = $(DE.roomRackInfo + ' ' + DE.uploadImage).get(0).files[0];
                if (uploadedfile == undefined || uploadedfile == null) {
                    alert(MultilingualKey.SI_OSP_GBL_GBL_GBL_099, 'warning');//Please select a file!
                    return false;
                }
                if (!action.validateImageFileType()) { return false; }
                frmData.append(uploadedfile.name, uploadedfile);
                frmData.append('system_Id', $(DE.roomRackInfo + ' ' + DE.liImage).data().systemId);
                frmData.append('entity_type', $(DE.roomRackInfo + ' ' + DE.liImage).data().entityType);
                ajaxReqforFileUpload('Main/UploadImage', frmData, true, function (resp) {
                    if (resp.status == "OK") {
                        action.getElementImages();
                        alert(resp.message, 'success', 'success');
                        if ($(DE.roomRackInfo + ' ' + DE.uploadImage)[0] != undefined)
                            $(DE.roomRackInfo + ' ' + DE.uploadImage)[0].value = '';
                    }
                    else {
                        alert(resp.message, 'warning');
                    }
                }, true);
            }
        },

        validateImageFileType: function () {
            var validFilesTypes = ["bmp", "gif", "png", "jpg", "jpeg"];
            var file = $(DE.roomRackInfo + ' ' + DE.uploadImage).val();
            var filepath = file;
            return action.validateFileType(validFilesTypes, filepath);
        },
        validateFileType: function (validFilesTypes, filepath) {
            var ext = filepath.substring(filepath.lastIndexOf(".") + 1, filepath.length).toLowerCase();
            var isValidFile = false;
            for (var i = 0; i < validFilesTypes.length; i++) {
                if (ext == validFilesTypes[i]) {
                    isValidFile = true;
                    break;
                }
            }
            if (!isValidFile) {
                //Invalid File. Please upload a File with
                alert(MultilingualKey.SI_OSP_GBL_JQ_FRM_059 +
                  " extension:\n\n" + validFilesTypes.join(", "), 'warning');
            }
            return isValidFile;
        },
        deleteEntityImages: function () {
            //Ready the list of selected images for delete
            var ListSystemIds = [];
            $.each($(".ImagesContainer  input[type='checkbox']"), function (indx, itm) {
                if ($(this).is(':checked')) {
                    ListSystemIds.push($(this).data().systemId)
                }
            });
            if (ListSystemIds.length > 0) {
                var func = function () {
                    ajaxReq('Main/DeleteEntityImages', { ListSystem_Id: ListSystemIds }, true, function (j) {
                        alert(j.message, 'success', 'success');
                        action.getElementImages();
                    }, true, true)
                };
                //Are you sure you want to delete selected images?
                showConfirm(MultilingualKey.SI_ISP_GBL_JQ_GBL_018, function () {
                    console.log('test');
                    func();
                });
            } else {
                alert(MultilingualKey.SI_ISP_GBL_JQ_GBL_019, 'warning');//Please select any image!
            }
        },

        downloadEntityImages: function (entityType) {
            //Ready the list of selected images for download
            var listPathName = [];
            $.each($(".ImagesContainer  input[type='checkbox']"), function (indx, itm) {
                if ($(this).is(':checked')) {
                    listPathName.push({ systemId: $(this).data().systemId });
                }
            });
            if (listPathName.length > 0) {
                window.location.href = 'DownloadFiles?json=' + JSON.stringify(listPathName) + '&entity_type=' + entityType;
            }
            else
                alert(MultilingualKey.SI_ISP_GBL_JQ_GBL_019, 'warning');//Please select any image!
        },


        getAttachmentFiles: function () {
            render.infoProgress(DE.roomRackInfo + ' ' + DE.divDocument);
            var _system_Id = $(DE.roomRackInfo + ' ' + DE.liDocument).data().systemId;
            var _entity_type = $(DE.roomRackInfo + ' ' + DE.liDocument).data().entityType;
            ajaxReq('ISP/getAttachmentDetails', { system_Id: _system_Id, entity_type: _entity_type }, true, function (jResp) {
                $(DE.roomRackInfo + ' ' + DE.divDocument).html(jResp);
            }, false, false, true);
           
            setTimeout(function () {
                $(".deleteRKDocs").each(function () {
                     
                    var cliks = $(this).attr("onclick");
                    cliks = cliks.replace("isp.getAttachmentFiles", "roomManager.getAttachmentFiles");
                    $(this).attr("onclick", cliks);
                });
            }, 1000);
        },

        validateDocumentFileType: function () {
            var validFilesTypes = ["dwg", "pdf", "jpeg", "jpg", "doc", "docx", "xls", "xlsx", "csv", "vsd", "ppt", "pptx", "png", "htm", "html"];
            var file = $(DE.roomRackInfo + ' ' + DE.uploadDocument).val();
            var filepath = file;
            return action.validateFileType(validFilesTypes, filepath);
        },

        deleteEntityDocuments: function (attachmentId) {
            //Ready the list of selected images for delete
            var ListSystemIds = [];
            $.each($("#ulDocumentUpload  input[type='checkbox']"), function (indx, itm) {
                if ($(this).is(':checked')) {
                    ListSystemIds.push($(this).data().systemId)
                }
            });
            if (ListSystemIds.length > 0) {
                var func = function () {
                    ajaxReq('Main/DeleteAttachmentFiles', { ListSystem_Id: ListSystemIds }, true, function (j) {
                        alert(j.message, 'success', 'success');
                        action.getAttachmentFiles();
                    }, true, true)
                };
                showConfirm(MultilingualKey.SI_OSP_GBL_JQ_FRM_004, func);//Are you sure you want to delete this file?
            } else {
                alert(MultilingualKey.SI_OSP_GBL_JQ_FRM_005, 'warning');//Please select any file!
            }
        },

        downloadEntityDocuments: function (entityType) {
            //Ready the list of selected images for download

            var listPathName = [];
            $.each($("#ulDocumentUpload  input[type='checkbox']"), function (indx, itm) {
                if ($(this).is(':checked')) {
                    listPathName.push({ systemId: $(this).data().systemId });
                }
            });
            if (listPathName.length > 0) {
                window.location.href = 'DownloadFiles?json=' + JSON.stringify(listPathName) + '&entity_type=' + entityType;
            } else {
                alert(MultilingualKey.SI_OSP_GBL_JQ_FRM_005, 'warning');//Please select any file!
            }
        },

        entityInformation: function (d) {
            if (validate.isRoomView()) {
                	
                if (d.model_type != undefined && d.model_type.toUpperCase() == 'EQUIPMENT') {
                    action.showEntityInformationDetail(d.db_id, 'Equipment', 'Equipment', 'POINT', false, d.network_status);
                } else {
                    action.showEntityInformationDetail(d.db_id, 'Rack', 'Rack', 'POINT', false, d.network_status);
                }
             

            }
            else { action.showEntityInformationDetail(d.db_id, 'Equipment', 'Equipment', 'POINT', false, d.network_status); }

            render.leftPnlToInfo();
        },
        setRoomId: function (roomId) {
            let $room = $(DE.selectedRoomId);
            if ($room && $room.length) {
                $room.val(roomId);
            }
        },

        selectFirstEquipment: function () {
            let selectEq = _workAreaData.filter(x=>x.model_id == DE.equipmentKey);
            if (selectEq && selectEq.length)
                action.selectModel(selectEq[0], true);
        },
        roomViewClick: function () {
            event.onLibFilterOption();
        },
        showOnRack: function (system_id) {
            _workAreaData = _workSpaceActions.getWorkArea();
            let model = _workAreaData.filter(x=>x.db_id == system_id);
            if (model.length) {
                render.modelFocus(model[0].id, true);
            }
        },
        getUnmappedFMS: function (parent_id, parent_entity_type) {

            var layer_name = $(DE.selectModelType).val();
            var layer_title = $(DE.selectModelType+" :selected").text();
             
            if (layer_name != "" && layer_name != null) {
                API.call(urls.getUnmappedFMS, { podId: parent_id, layer_name: layer_name, parent_entity_type: parent_entity_type }, function (res) {
                    let $fmsList = $(DE.selectExistedFms);
                    _unmappedRepsonse = res;
                    render.selector({
                        data: res,
                        element: $fmsList,
                        disabled: res.length == 0,
                        value: 'system_id',
                        text: 'DisplayNameWithPort',
                        defaultText: 'Select ' + layer_title,
                        dataKey: [{ key: 'network_id', name: 'data-network-id' }, { key: 'specification', name: 'data-specification' }, { key: 'item_code', name: 'data-item-code' }, { key: 'vendor_id', name: 'data-vendor-id' }, { key: 'no_of_port', name: 'data-no-of-port' }, { key: 'is_isp_mapped', name: 'data-is-isp-mapped' }]
                    });
                    $(DE.selectExistedFms + " option[data-is-isp-mapped='true']").prop("disabled", true);
                    let options = generate.libraryOptions();
                    render.filterLibList(options);
                });
            }

        },
        getMiddlewareType: function () {
            API.call(urls.getMiddlewareType, null, function (res) {
                 
                let $fmsList = $(DE.selectModelType);
                render.selector({
                    data: res,
                    element: $fmsList,
                    disabled: res.length == 0,
                    value: 'key',
                    text: 'value',
                    defaultText: 'Select Entity Type'
                    // dataKey: [{ key: 'network_id', name: 'data-network-id' }, { key: 'specification', name: 'data-specification' }, { key: 'item_code', name: 'data-item-code' }, { key: 'vendor_id', name: 'data-vendor-id' }, { key: 'no_of_port', name: 'data-no-of-port' }, { key: 'is_isp_mapped', name: 'data-is-isp-mapped' }]
                });
                // $(DE.selectModelType + " option[data-is-isp-mapped='true']").prop("disabled", true);

            });

        },
        validateEditByRights: function (networkStatus) {
            let entity = validate.isRoomView() ? 'rack' : 'equipment';
            return validate.editRights(entity, networkStatus);
        },
        validateDeleteByRights: function (networkStatus) {
            let entity = validate.isRoomView() ? 'rack' : 'equipment';
            return validate.deleteRights(entity, networkStatus);
        },
        validateViewByRights: function (networkStatus) {
            let entity = validate.isRoomView() ? 'rack' : 'equipment';
            return validate.viewRights(entity, networkStatus);
        },
        validateAddByRights: function (networkStatus) {
            let entity = validate.isRoomView() ? 'rack' : 'equipment';
            return validate.addRights(entity, networkStatus);
        },
        updateRackChildrenStatus(id, callback) {
            if (!validate.isRoomView()) {
            API.call(urls.getRackChildren, { rackId: id, parent_id: current_parent.systemId, parent_type: current_parent.entityType }, function (res) {
                //console.log(res, "load children");
                if (res.length > 0) {

                    if (typeof callback == 'function')
                        callback(res);
                }
            });
            }
        },
        updateEquipmentStatus: function (res) {
            let ports = res.filter(x=> x.model_id == DE.portKey);
            let len = ports.length;
            for (let i = 0; i < len ; i++) {
                let model = _workAreaData.filter(x=>x.db_id == ports[i].db_id);
                if (model.length)
                    model[0].port_status_color = ports[i].port_status_color;
            }
            render.allModels();
        },
        modelMove: function (model, position) {
            let modelFound = d3.select("#" + DE.modelElementId + model.id);
            let modelGroupFound = d3.select("#" + DE.modelGroup + model.id);
            if ((model.parent == -1)) {
                if (position.x > 0 && position.y > 0) {
                    model.position.x = position.x;
                    model.position.y = position.y;
                    //modelFound.attr("x", model.position.x).attr("y", model.position.y);
                    modelGroupFound.attr("transform", function () {

                        return 'translate(' + generate.modelPositionX(model) + ',' + generate.modelPositionY(model) + ') rotate(' + model.rotation_angle + "," + model.width / 2 + "," + model.height / 2 + ")";
                    });
                }
            }
            else {

                let param = generate.inBoundaryPos(model.parent, model, position);
                //Calculate param using rotation
                let collisionParam = { x: position.x, y: position.y };
                if (validate.inBoundary(model.parent, param) && !validate.collision(model.id, collisionParam)) {
                    model.position.x = position.x;
                    model.position.y = position.y;

                    //modelFound.attr("x", model.position.x).attr("y", model.position.y);
                    modelGroupFound.attr("transform", 'translate(' + generate.modelPositionX(model) + ',' + generate.modelPositionY(model) + ') rotate(' + model.rotation_angle + "," + model.width / 2 + "," + model.height / 2 + ")");
                }
                //let reLPos = generate.modelRealPos(model, model.rotation_angle);
                let rect = _workSpaceActions.getAbsoluteRect(model.parent);
                render.tempModel({

                    position: { x: position.x, y: position.y },
                    height: model.height,
                    width: model.width,
                    content: modelFound.html(),
                    rotation_angle: model.rotation_angle,
                    id: model.id,
                    parentPosition: { x: rect.x, y: rect.y },
                    parent: model.parent
                });
                render.equipmentStatus(model);
            }
        },
        selectedModelMove: function (d) {
            let model = d;
            if (!validate.isRoomView()) {
                if (model && model.id) {
                    model = _workSpaceActions.getSelectedParent(model.id, DE.equipmentKey);
                }
                if (!model) {
                    model = d;
                }

            }
            if (model && model.id) {

                model = _workSpaceActions.getDraggable(model.id);
                if (model && !model.is_static) {
                    let position = { x: 0, y: 0 };

                    let shift = generate.positionShift();
                    position.x = shift.x + model.position.x;
                    position.y = shift.y + model.position.y;
                    if (Math.abs(shift.x) > 0 || Math.abs(shift.y) > 0) { action.modelMove(model, position); }
                    else { render.resetTempModel(); }
                }
            }
        },
        showConnectionWire: function (ports) {
            _connectionView = true;
            _workAreaData = _workSpaceActions.getWorkArea();
            let _rackData = _workAreaData[0];
            let portRect = _workSpaceActions.getAbsoluteRect(_selectedModel.id);
            portRect.x = (portRect.x + portRect.width / 2) - generate.modelPositionX(_rackData);
            portRect.y = (portRect.y + portRect.height / 2) - generate.modelPositionY(_rackData);
            let container = d3.select('#' + DE.modelGroup + '1');
            container = container.append("svg")
                   .attr("pointer-events", "none")
                   .attr("id", DE.ElementLine)
                   .attr("height", _rackData.height)
                   .attr("width", _rackData.width);
            for (var i = 0; i < ports.length; i++) {
                let _tagert = _workAreaData.filter(x=>x.db_id == ports[i].destination_system_id && x.model_id == ports[i].model_id);
                if (_tagert != null && _tagert.length > 0) { render.showConnectedWire(_tagert[0], ports[i].port_type); }

            }
            let circleContainer = container.append("g");
            circleContainer.append('circle')
              .attr('fill', 'green')
              .attr('cx', function () { return portRect.x; })
              .attr('cy', function () { return portRect.y; })
              .attr('r', 4);

            if (validate.isOutConnectionFound(ports)) {
                render.showOutConnection();
            }
        },
        isConnectionView: function () {
            return _connectionView;
        },
        equipmentView : function (_systemId,_networkId,_rackdData) {
            ajaxReq('ISP/CreateModel', {
                modelId: _systemId
            }, false, function (resp) {
                $("#divEquipmentView").html(resp).show();
                $('#dvEquipmentNetworkId').html(_networkId);
                $('#hdnRackWidth').val(_rackdData.width);
                $('#hdnRackHeight').val(_rackdData.height);
                $('#RoomView').hide();
            }, false, false);
        },

        getRefLinksFiles: function () {
            render.infoProgress(DE.roomRackInfo + ' ' + DE.divRefLink);
            var _system_Id = $(DE.roomRackInfo + ' ' + DE.liRefLink).data().systemId;
            var _entity_type = $(DE.roomRackInfo + ' ' + DE.liRefLink).data().entityType;
            ajaxReq('ISP/getISPEntityRefLink', { system_Id: _system_Id, entity_type: _entity_type }, true, function (jResp) {
                jResp = jResp.replace('id="divDeleteImages"', 'id="divDeleteImagesroom"');
                $(DE.roomRackInfo + ' ' + DE.divRefLink).html(jResp);
            }, false, false, true);
            setTimeout(function () {
                $("#uploadRefLinkupload").attr("onclick", "roomManager.uploadReferenceLink()");             
                $(".deleteRefLnks").each(function () {
                     
                    var cliks = $(this).attr("onclick");
                    cliks = cliks.replace("isp.getRefLinksFiles", "roomManager.getRefLinksFiles");
                    $(this).attr("onclick", cliks);
                });
            }, 1000);
        },

        uploadReferenceLink: function (_systemId, _entityType) {
            popup.LoadModalDialog('PARENT', 'ISP/uploadReferenceLinks', { system_Id: 0 }, 'Upload Reference Link', "modal-sm");
            setTimeout(function () {
                $("#btnSaveRefLink").attr("onclick", "roomManager.uploadRefLinkRoom()");
            }, 1000);
        },
        uploadRefLinkRoom: function () {
             
            var isvalidFm = true;
            if ($('#DocrefLinkText').val().trim() == "") {
                $('#DocrefLinkText').addClass("notvalid notvalid input-validation-error");
                isvalidFm = false;
        }
            else {
                $('#DocrefLinkText').removeClass("notvalid notvalid input-validation-error");
            }
            if ($('#DocrefLink').val().trim() == "") {
                $('#DocrefLink').addClass("notvalid notvalid input-validation-error");
                isvalidFm = false;
            }
            else {
                $('#DocrefLink').removeClass("notvalid notvalid input-validation-error");
            }

            if (isvalidFm) {
                var url = $("#DocrefLink").val().trim();
                var pattern = /^(http|https)?:\/\/[a-zA-Z0-9-\.]+\.[a-z]{2,4}/;
                if (pattern.test(url)) {
                    var frmData = new FormData();
                    render.infoProgress(DE.roomRackInfo + ' ' + DE.divRefLink);
                    var _system_Id = $(DE.roomRackInfo + ' ' + DE.liRefLink).data().systemId;
                    var _entity_type = $(DE.roomRackInfo + ' ' + DE.liRefLink).data().entityType;

                    frmData.append('system_Id', _system_Id);
                    frmData.append('entity_type', _entity_type);
                    frmData.append('refDisplayTxt', $('#DocrefLinkText').val());
                    frmData.append('refLink', $('#DocrefLink').val());
                    ajaxReqforFileUpload('Main/UploadRefLink', frmData, true, function (resp) {
                         
                        if (resp.status == "OK") {
                            $('#closeModalPopup').trigger("click");
                            action.getRefLinksFiles();
                            alert(resp.message);
                        }
                        else {
                            alert(resp.message);
                        }

                    }, true);
                }
                else {
                    alert("Invalid reference link! <br> (Ex: 'http or https://www.xyz.com')");
                }
            }
            else {
                return false;
            }
        }             
    };

    //var bindEvent = {
    //    portMouseOver: function () {
    //        $(document).on('mouseover', DE.portRect, event.onPortMouseOver);
    //    }
    //};

    var event = {
       
        onIspTabClick: function (e) {
             
            _workAreaData = _workSpaceActions.getWorkArea();
            var rackData = _workAreaData.filter(x=>x.db_id > 0);
            if (!validate.isRoomView()) {
                rackData = _roomAllData.filter(x=>x.db_id > 0);
            }
            var rackCount = rackData.filter(x=>x.db_id > 0 && x.db_id != 999999 && x.model_type != 'Equipment').length;
            var entity_name = current_parent.entityType;
            var entity_id = current_parent.systemId;
            if (entity_name.toUpperCase() == 'FLOOR') {
                var $rackEntity = $(DE.FloorInfoBOX + "[data-system-id=" + entity_id + "]");
                if (rackCount > 0) {
                    $rackEntity.find(DE.RackDisplay).show();
                } else {
                    $rackEntity.find(DE.RackDisplay).hide();
                }
                $rackEntity.find(DE.EntityRackCount).text(rackCount);
            } else {
                var $ele = $("#div_" + entity_name.toUpperCase() + "_" + entity_id + "").find("span");
                var unitText = $ele.text().split(',');
                if (rackCount > 0) {
                    $ele.text(unitText[0] + ", Racks : " + rackCount);
                } else {
                    $ele.text(unitText[0]);
                }               
            }
            //console.log("rackCount:" + rackCount + " entity_name:" + entity_name + " entity_id:" + entity_id);
            let $e = $(e.target);
            render.leftPanel($e.data('room'));
            $('#rackViewDisplay > .svgDiv').show();
        },
        onRoomViewClick: function (e) {
             
             action.clear();
            load.libArea();
            _isRoomView = true;
            action.setScale();
            if (e && e.target) {
                //get current parent information  

                let $current = $(e.target);
                let currentData = $current.data();

                //get main parent
                let $pod = $('#div_' + currentData.entityType + '_' + currentData.systemId);
                current_parent.systemId = currentData.systemId;
                current_parent.entityType = currentData.entityType;
                action.setRoomId(currentData.systemId);
                render.setPopDetail(current_parent);
                // Get pod daTA
                //let podData = $pod.data();
                //let podName = podData.entityName;
                //let parantType = podData.parentEntityType;
                //let parantId = podData.parentSystemId;
                //if (parantType.toLowerCase() == 'structure') {
                //    parantType = 'floor';
                //    let floorId = podData.floorId;
                //    parantId = floorId;
                //}
                //current_parent.networkId = podData.networkId;
                //render.setRoomTitle(current_parent.networkId + "(" + podName + ")");
                //call room init
                //alert("Loading");
                API.call(urls.getRoomDetail, { id: current_parent.systemId, type: current_parent.entityType }, function (res) {
                    action.initRoom(res, current_parent.entityType);
                    event.onSearchLibraryData();
                });
            }
            else {

                let popId = $(DE.ispPOPId).val();
                let popType = $(DE.ispPOPType).val();
                current_parent.systemId = popId;
                current_parent.entityType = popType;
                action.setRoomId(current_parent.systemId);

                API.call(urls.getRoomDetail, { id: current_parent.systemId, type: current_parent.entityType }, function (res) {
                    action.initRoom(res, current_parent.entityType);
                    event.onSearchLibraryData();
                });
            }
            action.getUnmappedFMS(current_parent.systemId, current_parent.entityType);
            render.rackView(false);
            render.rackViewFooter(false);

            render.workAreaSwitch();
            render.scaleText(MultilingualKey.SI_ISP_GBL_GBL_GBL_053);
            $('.tooltip').hide();
            $(DE.sourceType).val(current_parent.entityType == "POD" ? "POP" : current_parent.entityType);
        },
        onRoomViewCloseClick: function (e) {
            render.roomView(false);
            render.leftPanel(false);
            render.rackViewFooter(false);
            $("#closeEquipmentView").trigger("click");
        },
        onWorkspaceContextMenu: function (e) {
            render.showSubMenuContext(false);
            //action.resetMultiSelection();
            //render.modelFocus();
            d3.event.preventDefault();
            d3.event.stopPropagation();
        },
        onLibAddButton: function () {
             
            let $current = d3.select(this);
            let model_id = parseInt($current.attr(DE.dataModelId));
            if (!validate.isNoNewEquipment()) {
                let name = generate.currentName();
                let newModel = generate.currentName(model_id);


                alert($.validator.format(MultilingualKey.SI_OSP_GBL_JQ_GBL_075, name, newModel), 'warning');
                return false;
            }
            let id = parseInt($current.attr(DE.libraryId));
            let img_id = parseInt($current.attr(DE.libraryImgId)) || '';
            let content = validate.isRoomView() ? $current.html().trim() : '';
            let db_height = parseFloat($current.attr("data-height"));
            let db_width = parseFloat($current.attr("data-width"));
            let db_depth = parseFloat($current.attr("data-depth"));
            let db_border_width = parseFloat($current.attr("data-border-width"));

            let model_type_id = parseInt($current.attr(DE.dataModelTypeId));
            let color = $current.attr(DE.dataColorCode);
            let stroke = $current.attr(DE.dataStrokeCode);
            let no_of_units = $current.attr(DE.dataUnitsNo);
            let height = parseFloat($current.attr("data-height"));
            let width = parseFloat($current.attr("data-width"));
            let depth = parseFloat($current.attr("data-depth"));
            let border_width = parseFloat($current.attr("data-border-width"));
            let name = $current.attr("data-name");
            let border_color = $current.attr("data-border-color");
            let isViewEnabled = false;
            if (validate.isRoomView()) {
                height = generate.meterToPixel(db_height);
                width = generate.meterToPixel(db_width);
                depth = generate.meterToPixel(db_depth);
                border_width = generate.meterToPixel(db_border_width);
                isViewEnabled = DE.roleAccess.rack.planned_view || false;
            }
            else {
                isViewEnabled = DE.roleAccess.equipment.planned_view || false;
            }
            action.selectModel(action.addModel({
                id: id,
                img_id: img_id,
                content: content,
                height: height,
                width: width,
                model_id: model_id,
                model_type_id: model_type_id,
                model_type: 'Rack',
                color: color,
                stroke: stroke,
                border_width: border_width,
                name: name,
                no_of_units: no_of_units,
                depth: depth,
                db_height: db_height,
                db_width: db_width,
                db_depth: db_depth,
                db_border_width: db_border_width,
                is_view_enabled: isViewEnabled,
                border_color: border_color

            }), true);


            _workAreaData = _workSpaceActions.getWorkArea();
            render.lnkRackView(validate.isRoomView() && _workAreaData.length > 1);
            render.lnkConnection(_workAreaData.length > 1);
            render.allModels();
            API.call(urls.getWorkspaceData, { modelId: id }, render.libChildren);

        },
        onLibAddEquipmentButton: function () {
             
            let $current = d3.select(this);
            let model_id = parseInt($current.attr(DE.dataModelId));
            if (!validate.isNoNewEquipment()) {
                let name = generate.currentName();
                let newModel = generate.currentName(model_id);


                alert($.validator.format(MultilingualKey.SI_OSP_GBL_JQ_GBL_075, name, newModel), 'warning');
                return false;
            }
            let id = parseInt($current.attr(DE.libraryId));
            let img_id = parseInt($current.attr(DE.libraryImgId)) || '';
            let content = validate.isRoomView() ? $current.html().trim() : '';
            let db_height = parseFloat($current.attr("data-height"));
            let db_width = parseFloat($current.attr("data-width"));
            let db_depth = parseFloat($current.attr("data-depth"));
            let db_border_width = parseFloat($current.attr("data-border-width"));

            let model_type_id = parseInt($current.attr(DE.dataModelTypeId));
            let model_type = $current.attr("data-model-type");
            let color = $current.attr(DE.dataColorCode);
            let stroke = $current.attr(DE.dataStrokeCode);
            let no_of_units = $current.attr(DE.dataUnitsNo);
            let height = parseFloat($current.attr("data-height"));
            let width = parseFloat($current.attr("data-width"));
            let depth = parseFloat($current.attr("data-depth"));
            let border_width = parseFloat($current.attr("data-border-width"));
            let name = $current.attr("data-name");
            let border_color = $current.attr("data-border-color");
            let isViewEnabled = false;
            if (validate.isRoomView()) {
                height = generate.mmToPixel(db_height);
                width = generate.mmToPixel(db_width);
                depth = generate.mmToPixel(db_depth);
                border_width = generate.mmToPixel(db_border_width);
                isViewEnabled = DE.roleAccess.rack.planned_view || false;
            }
            else {
                isViewEnabled = DE.roleAccess.equipment.planned_view || false;
            }
            action.selectModel(action.addModel({
                id: id,
                img_id: img_id,
                content: content,
                height: height,
                width: width,
                model_id: model_id,
                model_type_id: model_type_id,
                model_type: model_type,
                color: color,
                stroke: stroke,
                border_width: border_width,
                name: name,
                no_of_units: no_of_units,
                depth: depth,
                db_height: db_height,
                db_width: db_width,
                db_depth: db_depth,
                db_border_width: db_border_width,
                is_view_enabled: isViewEnabled,
                border_color: border_color

            }), true);


            _workAreaData = _workSpaceActions.getWorkArea();
            render.lnkRackView(validate.isRoomView() && _workAreaData.length > 1);
            render.lnkConnection(_workAreaData.length > 1);
            render.allModels();
            //API.call(urls.getWorkspaceData, { modelId: id }, render.libChildren);

        },
        onStartModelDrag: function (d) {
            //if (d.is_editable) {
            render.showSubMenuContext(false);
            if (d3.event.sourceEvent.shiftKey && d.id > 1) {
                //console.log(d.id);
                if (_selectedModel.id && _selectedModel.id > 1) {
                    if (!_selectMany.includes(_selectedModel.id))
                        _selectMany = _workSpaceActions.selectMany(_selectedModel.id);

                }
                if (!_isPositionChanged) {
                    action.selectModel({});
                    if (_selectMany.length == 0) {
                        render.modelFocus();
                    }
                }
                //if (!_selectMany.includes(d.id)) {
                //    _selectMany = _workSpaceActions.selectMany(d.id);
                //    render.modelFocus(d.id, false);
                //}
                //else {
                //    _selectMany.splice(_selectMany.indexOf(d.id), 1);
                //    render.modelUnFocus(d.id);
                //}
                //if (_selectMany.length == 1) {
                //    action.selectModel(_workSpaceActions.select(_selectMany[0]));
                //}
            }
            let model = d;
            //Get draggable Parent if model is in static mode
            if (!validate.isRoomView()) {
                model = _workSpaceActions.getSelectedParent(model.id, DE.equipmentKey);
                if (!model) {
                    model = d;
                }
            }
            if (model) {
                if (!model.is_static) {
                    let rect = _workSpaceActions.getAbsoluteRect(model.id);
                    model.offset_x = model.position.x - d3.event.x - (model.id != d.id ? rect.x : 0);
                    model.offset_y = model.position.y - d3.event.y - (model.id != d.id ? rect.y : 0);
                    //console.log("x:=" + d3.event.x + " y:=" + d3.event.y);
                }
            }
            //If Model is static then drag parent model if exist
            //render.resetResizeControl();
            //if (_libraryAllData.length && model.id == 1) {
            //    render.resetRuler();
            //}
            //}
        },
        onModelDrag: function (d) {
            //if (d.is_editable) {
            let model = d;
            if (!validate.isRoomView()) {
                model = _workSpaceActions.getSelectedParent(model.id, DE.equipmentKey);
                if (!model) {
                    model = d;
                }
            }
            if (model) {
                //let modelFound = d3.select("#" + DE.modelElementId + model.id);
                //let modelGroupFound = d3.select("#" + DE.modelGroup + model.id);


                if (!model.is_static) {
                    let position = { x: 0, y: 0 };
                    let rectModel = _workSpaceActions.getAbsoluteRect(model.id);
                    position.x = d3.event.x + model.offset_x + (model.id != d.id ? rectModel.x : 0);
                    position.y = d3.event.y + model.offset_y + (model.id != d.id ? rectModel.y : 0);

                    action.modelMove(model, position);
                }
            }
            // }
        },
        onEndModelDrag: function (d) {
            //if (d.is_editable) {
            let model = d;
            if (!validate.isRoomView()) {
                model = _workSpaceActions.getSelectedParent(model.id, DE.equipmentKey);
                if (!model) {
                    model = d;
                }
            }
            if (model) {
                if (!model.is_static) {
                    model.offset_x = 0;
                    model.offset_y = 0;

                    //Check drop container model and attached to it
                    //validate.whereDropped(model);
                    render.resetTempModel();
                    render.equipmentStatus(model);
                    //if (_libraryAllData.length) {
                    //    render.ruler(_workAreaData[0]);

                    //}
                }
            }
            //}
        },
        onModelContextMenu: function (d) {

            //render.showSavePos(d.db_id > 0);
            let model = d;
            if (!validate.isRoomView()) {
                model = _workSpaceActions.getSelectedParent(model.id, DE.equipmentKey);
                if (model)
                    render.showConnectionMenu((model.db_id > 0 && model.is_internal_connectivity_enabled));
                else {
                    model = d;
                    render.showConnectionMenu(false);
                }
                render.showEditorSubMenuContext(d.db_id > 0);
            }
            if (model) {

                render.modelSaveText(model.db_id == 0);
                if (model.is_view_enabled && model.id != 1 && model.is_editable) {
                    if (_selectedModel.id != model.id) {
                        action.cancelRackEdit();
                        //console.log(d);
                        _selectedModel = _workSpaceActions.select(model.id);
                        action.selectModel(_workSpaceActions.select(model.id));
                    }
                    let coordinates = [];
                    //coordinates = d3.mouse(this);
                    coordinates = [d3.event.x, d3.event.y];
                    _selectMany = _workSpaceActions.selectMany();
                    render.subContextMenu(model, coordinates, _selectMany.length > 1);
                    render.modelFocus(model.id);                    
                }
            }
            d3.event.preventDefault();
            d3.event.stopPropagation();
        },
        onModeldblClick: function (d) {

            //Draw outer boundery add circle in corner
            if (d.is_view_enabled && d.is_editable) {

                if (_selectedModel && _selectedModel.db_id > 0) {
                    action.cancelRackEdit();
                }

                if (_workSpaceActions.newModelCount() > 0 && d.id > 1 && d.db_id == 0) {
                    action.selectModel(d);
                    render.modelFocus(d.id);
                }
                else if (_workSpaceActions.newModelCount() == 0 && d.id > 1 && d.db_id > 0) {
                    action.selectModel(d);
                    render.modelFocus(d.id);
                    if (d.id > 1 && d.db_id > 0) {
                        event.onEntityInformation(d);
                    }
                }
                if (_workSpaceActions.newModelCount() > 0 && d.id > 1 && d.db_id > 0) {
                    let name = generate.currentName();

                    alert($.validator.format(MultilingualKey.SI_OSP_GBL_JQ_GBL_077, name), 'warning');//Please save/remove recently added
                }
                //Prevent all default events
                d3.event.preventDefault();
                d3.event.stopPropagation();

            }
        },
        onLibFilterOptionClick: function (e) {
            let radioValue = $(DE.libFilterOptions + ":checked").val();
            let options = _rackLibraryData.filter(x=> x.model_id == radioValue);

            //generate.libraryList(options);
            //render.library();
            render.showEquipmentType(radioValue == DE.equipmentKey);
            render.showFMSMap(false);
            if (radioValue == DE.FMSMap) {
                action.getMiddlewareType();

                //action.getUnmappedFMS(current_parent.systemId,current_parent.entityType);
                render.showFMSMap(true);
            }
            let $type = $(DE.selectLibTypes);
            $type.val('');
            if (options && options.length && _equipmentTypeData && _equipmentTypeData.length) {
                let modelType = $(DE.selectModelType).val();
                //let libTypes = _equipmentTypeData.filter(x=>x.key.toLowerCase() != 'fms')modelType.toLowerCase()
               //  
                //let keyType = current_parent.entityType.toUpperCase() == 'UNIT' || current_parent.entityType.toUpperCase() == 'FLOOR' ? 'htb' : 'fms';
                //let libTypes = _equipmentTypeData.filter(x=>x.key.toLowerCase() != keyType)
                let libTypes = _equipmentTypeData.filter(x=>x.is_middleware_model_type == false);
                render.selector({
                    data: libTypes,
                    element: $(DE.selectLibTypes),
                    disabled: libTypes.length == 0,
                    value: 'id',
                    text: 'value',
                    dataKey: [{ key: 'color', name: 'data-color-code' }]
                });
                $type.val(libTypes[0].id);
            }
           
            event.onChangeSelectLibTypes();
        },
        onRoomLibFilterOptionClick: function (e) {
            let isRackLib = $(DE.roomLibFilterOptions + ":checked").val();
            render.showRoomViewEquipmentLibrary(isRackLib);
        },
        onLibFilterOption: function () {
             
            let radioValue = DE.equipmentKey;
            let options = _rackLibraryData.filter(x=> x.model_id == radioValue);
            render.showRoomViewEquipmentType(radioValue == DE.equipmentKey);
         
            let $type = $(DE.selectRoomViewLibTypes);
            $type.val('');
            if (options && options.length && _equipmentTypeData && _equipmentTypeData.length) {
               
                let libTypes = _equipmentTypeData.filter(x=>x.is_middleware_model_type == false);
                render.selector({
                    data: libTypes,
                    element: $(DE.selectRoomViewLibTypes),
                    disabled: libTypes.length == 0,
                    value: 'id',
                    text: 'value',
                    dataKey: [{ key: 'color', name: 'data-color-code' }]
                });
                $type.val(libTypes[0].id);
            }

            event.onChangeSelectRoomViewLibTypes();
        },
        onChangeSelectRoomViewLibTypes: function () {
            $(DE.txtSearchRoomViewLibraryData).val('');
            let options = generate.libraryRoomViewOptions();
            render.filterRoomViewLibList(options);
        },

        onChangeSelectLibTypes: function (e) {
            $(DE.txtSearchLibraryData).val('');
            let options = generate.libraryOptions();
            render.filterLibList(options);
            //////////////
          let radioValue = $(DE.libFilterOptions + ":checked").val();
            // $("input[name=LibFilterCheck]").attr("value");
            var addscc = radioValue == "6" ? "258" : radioValue == "1" ? "315" : radioValue == "fmsMap" ? "370" : "";
            if (addscc != "") {
                $(".libscroll").css("max-height", "calc(100vh - " + addscc + "px)");
            }

        },
        onSearchLibraryData: function (e) {
            let options = generate.libraryOptions();
            render.filterLibList(options);
        },

        onSearchRoomViewLibraryData: function (e) {
            let options = generate.libraryRoomViewOptions();
            render.filterRoomViewLibList(options);
        },

        onlnkRackView: function (e) {
           
            $(DE.sourceType).val('Rack');
            //if (!action.validateViewByRights(_selectedModel.network_status))
            //{
            //    e.preventDefault();
            //    return false;
            //}

            load.libArea();
            if (_selectedModel.db_id == 0) {
                alert(MultilingualKey.SI_OSP_GBL_JQ_GBL_079, "warning");//Please save rack first to equip further!
                return false;
            }
            action.rackView();
            render.showSubMenuContext(false);
            render.modelFocus();
            render.showRoomEquipmentLib(false);
            $("#ParentType").val(current_parent.entityType);
            $("#ParentID").val(current_parent.systemId);
            $('.event a').text("Rack View");
        }, 
        onlnkRoomView: function (e) {
           
             
            $(DE.sourceType).val(current_parent.entityType);
            $(DE.dvDropbox).hide();
            $("#dvConnection").hide();
            load.libArea();
            render.resetWorkArea();
            _isRoomView = true;
            action.setScale();
            render.rackView(false);
            render.rackViewFooter(false);
            action.setRackChildren();

            render.workAreaSwitch();
            render.allModels();
            render.scaleText(MultilingualKey.SI_ISP_GBL_GBL_GBL_053);
            render.infoToLeftPnl();
            event.onSearchLibraryData();
            let sel_rack = _workAreaData.filter(x=>x.db_id == _rackIndex);
            action.selectModel(sel_rack[0]);
            render.showSubMenuContext(false);
            render.modelFocus();
            $('.event a').text("Room View");
        },
        onCancelRackSubMenu: function (e) {
            //action.resetMultiSelection();
            render.modelFocus();
            render.showSubMenuContext(false);
        },
        onSaveRackSubMenu: function (e) {
             
            if ((_selectedModel.db_id == 0 && !action.validateAddByRights(_selectedModel.network_status)) || (_selectedModel.db_id > 0 && !action.validateEditByRights(_selectedModel.network_status))) {
                e.preventDefault();
                return false;
            }
            if (validate.isRoomView()) {
                if (_selectedModel.model_type != undefined && _selectedModel.model_type.toUpperCase() == "EQUIPMENT") {
                    action.saveEquipment(e);
                }
                else {
                    action.saveRack(e);
                }
            }
            else { action.saveEquipment(e); }
            //render.modelFocus();
            render.showSubMenuContext(false);
        },
        onDeleteSubMenuContext: function (e) {

            if (_selectedModel.db_id != 0 && !action.validateDeleteByRights(_selectedModel.network_status)) {
                e.preventDefault();
                return false;
            }
            let name = generate.currentName();
            //Are you sure you want to delete this


            confirm($.validator.format(MultilingualKey.SI_OSP_DSA_JQ_FRM_003, name), action.remove);
            render.modelFocus();
            render.showSubMenuContext(false);
        },
        onClickRackTab: function (e) {
            $("#dvConnection").hide();
            let $tab = $(e.target);
            let id = $tab.data('dbId');
            _rackIndex = id;
            render.activeRackTab($tab);
            // console.log('rack tab click ', id);

            render.resetWorkArea();
            _isRoomView = false;
            action.setScale();
            render.rackView(true);
            render.rackViewFooter(true);
            action.setRackChildren();

            _workAreaData = [];
            if (_roomAllData.length > 1) {
                _rackAllData = _roomAllData.filter(x=> x.db_id == id)
                //_selectedModel = _rackAllData[0];
                action.selectModel(_rackAllData[0], true);
                generate.rackChildren(_rackAllData[0]);
                $.extend(true, _workAreaData, _rackAllData[0].children);


            }
            _workSpaceActions.setWorkArea(_workAreaData);

            render.workAreaSwitch();
           
            if (id == 999999) {
                var objDimension = action.getNoRackDimensions(_roomAllData);
                action.changeNoRackAreaScale(_workAreaData[0], objDimension);
                //render.setWorkSpaceSize(objDimension.height + 150, objDimension.width );
            }
            else {
                action.changeRackAreaScale(_workAreaData[0]);
            }
            render.setWorkSpaceSize(_workAreaData[0].height + 150, _workAreaData[0].width + (2 * _workAreaData[0].db_border_width) + 150);
            //render.setInTop(_workAreaData[0]);
            //action.selectFirstEquipment();
            render.allModels();

            action.getRackChildren(_workAreaData[0].db_id);
            render.infoToLeftPnl();
            event.onSearchLibraryData();
            render.modelFocus();
            if (id == 999999) {
                $(DE.navRoomLib).hide();
                $(DE.roomLeftPanel).hide();
                $(DE.rackAreaContext).addClass("withoutRack")
            }
            else {
                $(DE.navRoomLib).show();
                $(DE.roomLeftPanel).show();
                $(DE.rackAreaContext).removeClass("withoutRack")
            }
        },
        onClickEquipmentTab: function (e) {
            $("#dvConnection").hide();
            let $tab = $(e.target);
            let id = $tab.data('dbId');
            _rackIndex = id;
            render.activeRackTab($tab);
            // console.log('rack tab click ', id);

            render.resetWorkArea();
            _isRoomView = false;
            action.setScale();
            render.rackView(true);
            render.rackViewFooter(true);


            action.setEquipmentChildren();

            _workAreaData = [];
            if (_roomAllData.length > 1) {
                _rackAllData = _roomAllData.filter(x=> x.db_id == id)
                //_selectedModel = _rackAllData[0];
                action.selectModel(_rackAllData[0], true);
                generate.rackChildren(_rackAllData[0]);
                $.extend(true, _workAreaData, _rackAllData[0].children);


            }
            _workSpaceActions.setWorkArea(_workAreaData);

            render.workAreaSwitch();
            action.changeRackAreaScale(_workAreaData[0]);
            render.setWorkSpaceSize(_workAreaData[0].height + 150, _workAreaData[0].width + (2 * _workAreaData[0].db_border_width) + 150);
            //render.setInTop(_workAreaData[0]);
            //action.selectFirstEquipment();
            render.allModels();

            action.getRackChildren(_workAreaData[0].db_id);
            render.infoToLeftPnl();
            event.onSearchLibraryData();
            render.modelFocus();
        },
        onEntityInformation: function (d) {

            //API.call(urls.getISPEntityInformationDetail, {
            //    systemId: d.db_id,
            //    entityName: 'Rack',
            //    entityTitle: 'Rack',
            //    geomType: 'POINT',
            //    networkStatus: 'P'
            //}, function (res) {
            //    $(DE.entityInfo).html(res);
            //});
            $('.rackContext').closest('svg g svg g').find('svg').addClass('equipmentActive');
            if (d.db_id == 0) {
                let name = generate.currentName();

                alert($.validator.format(MultilingualKey.SI_ISP_GBL_JQ_GBL_050, name), 'warning');//Please save//first!
                return false;
            }
            action.entityInformation(d);
        },
        onEditRack: function (e) {
            action.editRack();
        },
        onCancelEditRack: function (e) {
            action.cancelRackEdit();
            //render.allModels();
        },
        onInfoRackView: function (e) {
            //$(DE.roomLeftPanel).show ();
            //$(DE.entityInfoPanel).hide();
            render.infoToLeftPnl();
            event.onlnkRackView();
        },
        onNavRoomLib: function (e) {
            render.infoToLeftPnl();
        },
        onNavRoomInfo: function (e) {
            render.leftPnlToInfo();
        },
        onEditSubMenu: function (e) {
            if (!action.validateEditByRights(_selectedModel.network_status)) {
                e.preventDefault();
                return false;
            }
            //action.resetMultiSelection();
            //render.modelFocus();
            render.showSubMenuContext(false);
            render.showConnectionView(false);
            action.editRack();
        },
        onSavePosition: function (e) {
            if (!action.validateEditByRights(_selectedModel.network_status)) {
                e.preventDefault();
                return false;
            }
            action.savePosition(e);
            render.modelFocus();
            render.showSubMenuContext(false);
        },
        onGetElementImages: function (e) {
            console.log("get images");
            action.getElementImages();
        },
        onUploadImageFile: function (e) {
            action.uploadImageFile();
        },
        onDeleteImages: function (e) {
            action.deleteEntityImages();
        },
        onDownloadImages: function (e) {
            action.downloadEntityImages($(DE.roomRackInfo + ' ' + DE.liImage).data().entityType);
        },
        onGetAttachmentFiles: function (e) {
            action.getAttachmentFiles();
        },
        onUploadDocument: function (e) { action.uploadDocumentFile(); },
        onDeleteDocument: function (e) { action.deleteEntityDocuments(); },

        onDownloadDocument: function (e) {

            action.downloadEntityDocuments($(DE.roomRackInfo + ' ' + DE.liImage).data().entityType);
        },
        onSelectUnmappedFMS: function (e) {
            $(DE.txtSearchLibraryData).val('');
            let options = generate.libraryOptions();
            render.filterLibList(options);
        },
        onModelTypeChange: function (e) {
             
            action.getUnmappedFMS(current_parent.systemId, current_parent.entityType);
            //let options = generate.libraryOptions();
           // render.filterLibList(options);
        },
        onRotateSubMenuContext: function (e) {

            if (!_selectedModel.is_static) {
                if (_selectedModel && _selectedModel.id > 1) {
                    let rotation = _selectedModel.rotation_angle;

                    _selectedModel.rotation_angle += 90;
                    if (_selectedModel.rotation_angle >= 360)
                        _selectedModel.rotation_angle = 0;


                    let param = generate.inBoundaryPos(_selectedModel.parent, _selectedModel, _selectedModel.position);

                    //let collisionParam = { x: position.x , y: position.y  };
                    if (validate.inBoundary(_selectedModel.parent, param) && !validate.collision(_selectedModel.id, _selectedModel.position)) {

                        render.allModels();
                    }
                    else {
                        _selectedModel.rotation_angle = rotation;
                        //Model can not be rotate here! Model boundary is colliding with other boundary!
                        alert(MultilingualKey.SI_OSP_GBL_JQ_GBL_080, "Warning");
                    }

                }
            }
            action.resetMultiSelection();
            render.modelFocus();
            render.showSubMenuContext(false);
        },
        onRevertPosSubMenuContext: function (e) {
            showConfirm(MultilingualKey.SI_OSP_GBL_JQ_GBL_081, function () {
                action.cancelRackEdit();
                render.showSubMenuContext(false);
            });

        },
        onConnectionCheck: function () {
            _connectionChecked = $(DE.chkShowConnections).is(':checked');
            action.updateRackChildrenStatus(_rackIndex, action.updateEquipmentStatus);


        },
        onConnectionViewContextMenu: function (e) {
            let conBuilder = new connectionBuilder();

            conBuilder.insideConnectivity(_selectedModel.db_id);
            render.modelFocus();
            render.showSubMenuContext(false);
        },
        onModelKeyPress: function () {
            _selectMany = _workSpaceActions.selectMany();
            if (_selectMany.length) {
                _workAreaData = _workSpaceActions.getWorkArea();
                for (let i = 0 ; i < _selectMany.length; i++) {
                    if (_selectMany[i] > 1) {
                        let index = _workSpaceActions.getIndex(_selectMany[i]);
                        if (index > -1)
                            action.selectedModelMove(_workAreaData[index]);
                    }
                }
            }
            else
                if (_selectedModel) {
                    action.selectedModelMove(_selectedModel);
                }
        },
        onDocumentKeyPress: function () {
            //let body = d3.select("document");
            //body.on("keydown", action.onModelKeyPress);
            d3.select('body')
                .on('keydown', function () {
                    _keyPressed[d3.event.keyCode] = true;
                    if (_selectedModel && (d3.event.keyCode === 37 || d3.event.keyCode === 38 || d3.event.keyCode === 39 || d3.event.keyCode === 40)) {
                        d3.event.preventDefault();
                        d3.event.stopPropagation();
                    }
                    //console.log(d3.event, d3.event.keyCode);
                })
                .on('keyup', function () {
                    _keyPressed[d3.event.keyCode] = false;
                    if (_selectedModel && (d3.event.keyCode === 37 || d3.event.keyCode === 38 || d3.event.keyCode === 39 || d3.event.keyCode === 40)) {
                        d3.event.preventDefault();
                        d3.event.stopPropagation();
                    }
                });
            d3.timer(event.onModelKeyPress);
        },
        onModelMouseOver: function (d) {
            if (d.model_id == DE.portKey && d.port_status_id == 2) {

                var _data = {
                    source_system_id: d.super_parent, source_network_id: d.short_network_id + '(' + d.name + ')', source_entity_type: 'Equipment', source_port_no: d.port_number,
                    portType: 'O'
                };
                let tooltip = document.getElementById(DE.dvConnection);
                tooltip.style.display = "block";
                let portRect = _workSpaceActions.getAbsoluteRect(d.id);
                var svgBoudary = document.getElementById(DE.modelElementId + d.id).getBoundingClientRect();
                tooltip.style.top = (svgBoudary.top + (d.height / 2) + 20) + 'px';
                tooltip.style.left = (svgBoudary.left + (d.width / 2) - 5) + 'px';// - $("#" + DE.dvConnection).width() + 'px';
                ajaxReq(urls.getPortMultipleConnections, _data, true, function (resp) {
                    $(DE.connectionDV).children(DE.divConnContainer).html(resp);
                    $(DE.connectionDV).css('background-image', 'none');
                }, false, false, true);
            }
        },
        onModelClick: function (d) {
            if (d.model_id == DE.portKey && d.port_status_id == 2) {
                $('#btnCloseMultiConnection').trigger("click");
                if (action.isConnectionView()) {
                    render.showConnectionView(false);
                } else {
                    render.modelFocus(d.id);
                    API.call(urls.getConnectedPorts, { systemId: d.super_parent, portNo: d.port_number }, function (res) {
                        if (res.length > 0) { action.selectModel(d); action.showConnectionWire(res); }
                    });
                }
            }
            d3.event.preventDefault();
            d3.event.stopPropagation();
        },
        onOutConnectionPointClick: function () {
            if (_selectedModel.model_id == DE.portKey && _selectedModel.port_status_id == 2) {
                var _data = {
                    source_system_id: _selectedModel.super_parent,
                    source_network_id: _selectedModel.short_network_id + '(' + _selectedModel.name + ')',
                    source_entity_type: 'Equipment', source_port_no: _selectedModel.port_number,
                    rackId: _selectedModel.rack_id,
                    isOutConnection: true
                };
                let tooltip = document.getElementById(DE.dvConnection);
                tooltip.style.display = "block";
                tooltip.style.top = d3.event.y + 'px';
                tooltip.style.left = d3.event.x + 'px';
                API.call(urls.getOutConnectedPorts, _data, function (resp) {
                    $(DE.connectionDV).children(DE.divConnContainer).html(resp);
                    $(DE.connectionDV).css('background-image', 'none');
                    hideProgress();
                });
            }
        },
        onWireMouseOver: function (d) {
            alert('this');
        },
        onWireMouseOut: function () { alert('this out'); },
        onEquipmentViewSubMenu: function (e) {
            let _workAreaData = _workSpaceActions.getWorkArea();
            action.equipmentView(_selectedModel.db_id, _selectedModel.network_id, _workAreaData[0]);
            $(DE.tabEquipmentView).show();
            render.setEquipmentViewTab();
        },
        ongetRefLinksFiles: function (e) {
            action.getRefLinksFiles();
        },
        onuploadRefLink: function (e) { action.uploadReferenceLink(); },
        onuploadSaveRefLink: function (e) { action.uploadRefLinkRoom(); },
        }

    var API = {
        call: function (url, param, callback) {
            ajaxReq(url, param, true, function (res) {
                hideProgress();
                if (callback && typeof callback == 'function')
                    callback(res);
            }, false, true, true);
        },

    };

    var generate = {
        modelTree: function (id, data, selected) {
            if (id == -1)
                return null;
            let children = data.filter(x=>x.parent == id);
            selected.children = [];
            for (let i = 0 ; i < children.length; i++) {
                //console.log(children[i]);
                selected.children.push(children[i]);
                generate.modelTree(children[i].id, data, children[i]);
            }

        },
        libraryList: function (req) {
            _libraryData = [];
            let len = req.length;
            let xOffset = 100, yOffset = 100;
            let pos_x = 0, pos_y = -yOffset, h = 100, w = 100, margin_x = 5, margin_y = 5;
            let content = '';
            let color = DE.rack_color;
            for (let i = 0; i < len; i++) {
                color = validate.isRoomView() ? DE.rack_color : req[i].color_code;
                if (i % 2 === 0) {
                    pos_x = margin_x;
                    pos_y += yOffset + margin_y;
                }
                content = req[i].image_data;
                //if (validate.isRoomView()) {
                //    content = rackGlobalData.svgContent;
                //    if ( req[i].model_id == DE.equipmentKey) {
                //        content = equipmentGlobalData.svgContent;
                //    }
                    
                //    //if (req[i].model_type != undefined && req[i].model_type.toUpperCase() == 'EQUIPMENT') {
                //    //    content = equipmentGlobalData.svgContent;
                //    //}

                //}
                //if (validate.isRoomView() ) {
                //    content = rackGlobalData.svgContent;
                //    if (req[i].model_id == DE.equipmentKey) {
                //        content = equipmentGlobalData.svgContent;
                //    }
                //} else if (!validate.isRoomView() && req[i].model_id == DE.equipmentKey)
                //    {
                //    content = rackGlobalData.svgContent;
                //}

                content = rackGlobalData.svgContent;
                if (req[i].model_id == DE.equipmentKey) {
                    content = equipmentGlobalData.svgContent;
                }
  

                _libraryData.push({
                    id: req[i].id,
                    position: { x: pos_x, y: pos_y },
                    height: h,
                    width: w,
                    image_data: content,
                    color: color,
                    stroke: req[i].stroke_code,
                    db_height: req[i].height, //(validate.isRoomView() ? req[i].height : generate.mmToPixel(req[i].height)),
                    db_width: req[i].width, // validate.isRoomView() ? req[i].width : generate.mmToPixel(req[i].width),
                    db_depth: req[i].depth, //validate.isRoomView() ? req[i].depth : generate.mmToPixel(req[i].depth),
                    db_border_width: req[i].border_width, //validate.isRoomView() ? req[i].border_width : generate.mmToPixel(req[i].border_width),
                    name: req[i].model_name,
                    db_id: req[i].id,
                    img_id: req[i].model_image_id,
                    model_id: req[i].model_id,
                    model_type_id: req[i].model_type_id,
                    model_type: req[i].model_type,
                    no_of_units: req[i].no_of_units,
                    db_color: color,
                    enabled: req[i].enabled == undefined ? true : req[i].enabled,
                    specification: req[i].specification,
                    code: req[i].code,
                    no_of_port: req[i].no_of_port,
                    vendor_id: req[i].vendor_id,
                    border_color: req[i].border_color

                });
                pos_x += xOffset + margin_x;

            }
        },
        pixelToMM: function (value) {
            return parseFloat((value * (_scaleFactor.grid_scale / _scaleFactor.gridCellSize)).toFixed(2));
        },
        mmToPixel: function (value) {
            return parseFloat((value * (_scaleFactor.gridCellSize / _scaleFactor.grid_scale)).toFixed(2));
        },
        rulerData: function (limit, offset) {
            let res = [];

            for (let i = 0; i <= limit; i += offset) {
                res.push(i);
            }
            return res;
        },
        currentWorkArea: function () {
            let $grid;
            if (validate.isRoomView()) {
                $grid = d3.select(DE.svgWorkArea);
            }
            else {
                $grid = d3.select(DE.svgRackArea);
            }
            return $grid;
        },
        currentWorkAreaContext: function () {
            let $grid;
            if (validate.isRoomView()) {
                $grid = $(DE.workAreaContext);
            }
            else {
                $grid = $(DE.rackAreaContext);
            }
            return $grid;
        },
        rackTabList: function () {
            _workAreaData = _workSpaceActions.getWorkArea();
            let rackTabList = [];

            return rackTabList;
        },
        rackChildren: function (rackData) {
            if (!rackData.children) { rackData.children = []; }
            if (rackData.children.length > 0) {
                rackData.children[0].height = rackData.children[0].depth;
            }
            if (!rackData.children.length) {
                rackData.children.push({
                    id: 1,
                    height: rackData.depth,
                    width: rackData.width,
                    depth: rackData.height,
                    offset_x: 0,
                    offset_y: 0,
                    db_id: rackData.db_id,
                    position: { x: rackData.width / 2, y: rackData.depth / 2 },// { x: 0, y: 0 },
                    rotation_angle: 0,
                    is_static: true,
                    border_width: rackData.border_width,
                    is_editable: true,
                    lib_id: 0,
                    parent: -1,
                    db_height: rackData.db_height,
                    db_width: rackData.db_width,
                    db_depth: rackData.db_depth,
                    no_of_units: rackData.no_of_units,
                    border_color: rackData.border_color,
                    db_border_width: rackData.db_border_width,
                    super_parent: rackData.super_parent,
                    port_number: rackData.port_number,
                    network_id: rackData.network_id,
                    port_status_id: rackData.port_status_id,
                    port_comment: rackData.port_comment
                });
            }

        },
        meterToPixel: function (value) {
            return generate.mmToPixel(value * _scaleFactor.displayScale);
        },
        //pixelToMeter: function (value) {
        //    return generate.pixelToMM(value) / _scaleFactor.displayScale;
        //},
        pixelToMeter: function (value) { return generate.pixelToMM(value) / 1000; },
        meterToMM: function (value) { return value * 1000; },
        doorPostion: function (param) {

            let doorData = generate.doorData(param);
            let position = { x: 0, y: 0, rotation: 0, svgContent: doorData.doorSvgContent[param.position], viewBox: doorData.viewBox, height: doorData.height, width: doorData.width };
            _workAreaData = _workSpaceActions.getWorkArea();
            if (_workAreaData.length > 0) {
                const room = _workAreaData[0];
                const roomRect = _workSpaceActions.getAbsoluteRect(room.id);
                if (param.position.length == 2) {
                    let wall = param.position[0];
                    let pos = param.position[1];
                    if (wall == 'E') {
                        switch (pos) {
                            case 'R':
                                position.x = roomRect.x + roomRect.width - position.width + 10;
                                position.y = roomRect.y + 10;
                                position.rotation = 0;
                                // position.svgContent = '<path d="M50,50 v-50 a50,50 0 0,0 -50,50z M0,49h50M0,48h50M0,47h50" fill="#bcbcbc" stroke="gray" stroke-width="1"/>';
                                //position.svgContent ='<rect width="51" height="46" fill="#E4E4E4"/><path fill-rule="evenodd" clip-rule="evenodd" d="M48 0H44C20.0191 0 0.519048 20.0567 0.0101912 45H0V46L1 46H44H48H51V45H48V0ZM1.01043 45L44 45V1.04545C20.5714 1.04545 1.5191 20.6341 1.01043 45Z" fill="#707070"/>';
                                break;
                            case 'M':
                                position.x = roomRect.x + room.width - position.width + 10;
                                position.y = roomRect.y + room.height / 2 - position.height / 2;
                                position.rotation = 0;
                                //position.svgContent = '<path d="M50,50 v-50 a50,50 0 0,0 -50,50 z M0,49h50M0,48h50M0,47h50" fill="#bcbcbc" stroke="gray" stroke-width="1"/>';
                                break;
                            case 'L':
                                position.x = roomRect.x + room.width - position.width + 10;
                                position.y = roomRect.y + room.height - position.height - 10;
                                position.rotation = 0;
                                //position.svgContent = '<path d="M50,0 h-50 a50,50 0 0,0 50,50 z M0,1h50M0,2h50M0,3h50" fill="#bcbcbc" stroke="gray" stroke-width="1"/>';
                                break;
                        }
                    }
                    if (wall == 'W') {
                        switch (pos) {
                            case 'R':
                                position.x = roomRect.x - 10;
                                position.y = roomRect.y + room.height - position.height - 10;
                                position.rotation = 0;
                                //position.svgContent = '<path d="M0,0 v50 a50,50 0 0,0 50,-50 z M0,1h50M0,2h50M0,3h50" fill="#bcbcbc" stroke="gray" stroke-width="1"/>';
                                break;
                            case 'M':
                                position.x = roomRect.x - 10;
                                position.y = roomRect.y + room.height / 2 - position.height / 2;
                                position.rotation = 0;
                                //position.svgContent = '<path d="M0,0 v50 a50,50 0 0,0 50,-50 z M0,49h50M0,48h50M0,47h50" fill="#bcbcbc" stroke="gray" stroke-width="1"/>';
                                break;
                            case 'L':
                                position.x = roomRect.x - 10;
                                position.y = roomRect.y + 10;
                                position.rotation = 0;
                                //position.svgContent = '<path d="M0,50 h50 a50,50 0 0,0 -50,-50 z M0,49h50M0,48h50M0,47h50" fill="#bcbcbc" stroke="gray" stroke-width="1"/>';
                                break;
                        }
                    }
                    if (wall == 'N') {
                        switch (pos) {
                            case 'R':
                                position.x = roomRect.x + 10;
                                position.y = roomRect.y - 10;
                                position.rotation = 0;
                                // position.svgContent = '<path d="M50,0 h-50 a50,50 0 0,0 50,50 z M47,0v50M48,0v50M49,0v50 " fill="#bcbcbc" stroke="gray" stroke-width="1"/>';
                                break;
                            case 'M':
                                position.x = roomRect.x + room.width / 2 - position.width / 2;
                                position.y = roomRect.y - 10;
                                position.rotation = 0;
                                //position.svgContent = '<path d="M50,0 h-50 a50,50 0 0,0 50,50 z M47,0v50M48,0v50M49,0v50" fill="#bcbcbc" stroke="gray" stroke-width="1"/>';
                                break;
                            case 'L':
                                position.x = roomRect.x + room.width - position.width - 10;
                                position.y = roomRect.y - 10;
                                position.rotation = 0;
                                //position.svgContent = '<path d="M0,0 v50 a50,50 0 0,0 50,-50 z M1,0v50M2,0v50M3,0v50" fill="#bcbcbc" stroke="gray" stroke-width="1"/>';
                                break;
                        }
                    }
                    if (wall == 'S') {
                        switch (pos) {
                            case 'R':
                                position.x = roomRect.x + room.width - position.width - 10;
                                position.y = roomRect.y + room.height - (position.height) + 10;
                                position.rotation = 0;
                                //position.svgContent = '<path d="M0,50 h50 a50,50 0 0,0 -50,-50 z M1,0v50M2,0v50M3,0v50" fill="#bcbcbc" stroke="gray" stroke-width="1"/>';
                                //position.svgContent = '<rect width="50" height="50" fill="#E4E4E4"/><path fill-rule="evenodd" clip-rule="evenodd" d="M48 0H44C20.0191 0 0.519048 20.0567 0.0101912 45H0V46L1 46H44H48H51V45H48V0ZM1.01043 45L44 45V1.04545C20.5714 1.04545 1.5191 20.6341 1.01043 45Z" fill="#707070"/>';

                                break;
                            case 'M':
                                position.x = roomRect.x + room.width / 2 - position.width / 2;
                                position.y = roomRect.y + room.height - position.height + 10;
                                position.rotation = 0;
                                //position.svgContent = '<path d="M0,50 h50 a50,50 0 0,0 -50,-50 z M1,0v50M2,0v50M3,0v50" fill="#bcbcbc" stroke="gray" stroke-width="1"/>';
                                break;
                            case 'L':
                                position.x = roomRect.x + 10;
                                position.y = roomRect.y + room.height - (position.height) + 10;
                                position.rotation = 0;
                                //position.svgContent = '<path d="M50,50 v-50 a50,50 0 0,0 -50,50 z M47,0v50M48,0v50M49,0v50" fill="#bcbcbc" stroke="gray" stroke-width="1"/>';
                                break;
                        }
                    }

                    //console.log("Door position : ", position);
                }
            }
            return position;
        },
        doorData: function (param) {
            let result = {
                doorSvgContent: '',
                viewBox: '0 0 50 50',
                height: param.width,
                width: param.width
            };


            //let viewBox = "0 0 50 50";
            let height = param.width, width = param.width;
            if (param.type.toLowerCase() == doorType.flushed.toLowerCase()) {
                result.doorSvgContent = flushedDoorSvgContent;
                result.viewBox = "0 0 50 50";
                result.height = param.width;
                result.width = param.width;
            }
            if (param.type.toLowerCase() == doorType.slide.toLowerCase()) {
                result.doorSvgContent = slideDoorSvgContent;
                result.viewBox = "0 0 85 9";
                result.height = 9;
                result.width = param.width;
                if (param.position[0] == 'E' || param.position[0] == 'W') {
                    result.viewBox = "0 0 9 85";
                    result.height = param.width;
                    result.width = 9;
                }

            }
            if (param.type.toLowerCase() == doorType.shutter.toLowerCase()) {
                result.doorSvgContent = shutterDoorSvgContent;
                result.viewBox = "0 0 80 16";
                result.height = 11;
                result.width = param.width;
                if (param.position[0] == 'E' || param.position[0] == 'W') {
                    result.viewBox = "0 0 16 80";
                    result.height = param.width;
                    result.width = 16;
                }

            }
            return result;
        },
        currentName: function (modelId) {
            modelId = !modelId ? _selectedModel.model_id : modelId;
            let name = 'Rack';
            if (validate.isRoomView()) {
                name = 'Rack';
                if (_selectedModel.model_type != undefined && _selectedModel.model_type.toUpperCase() == 'EQUIPMENT') {
                    name = 'Equipment';
                }
                
            }
            else {

                switch ('' + modelId) {
                    case DE.equipmentKey:
                        name = 'Equipment';
                        break;
                    case DE.trayKey:
                        name = 'Tray';
                }
            }
            return name;
        },
        unitData: function (limit, offset) {
            let res = [];

            for (let i = 1; i <= limit; i += offset) {
                res.push(i);
            }
            return res;
        },
        toBoolean: function (attr) {
            return JSON.parse(attr.toLowerCase());
        },
        noOfPorts: function (parent) {
            let noOfPorts = 0;
            _workAreaData = _workSpaceActions.getWorkArea();
            let totalPorts = _workAreaData.filter(x=>x.model_id == DE.portKey && x.parent == parent);
            noOfPorts = totalPorts.length;
            return noOfPorts;
        },
        libraryRoomViewOptions: function () {
             
            let options = _libraryAllData;
            if (validate.isRoomView()) {
                 
                let radioValue = 1;
  
                let fmsType = _equipmentTypeData.filter(x=>x.is_middleware_model_type == false);
                options = _rackLibraryData.filter(x=> x.model_id == radioValue);

            } 
            return options;
        },
        libraryOptions: function () {
             
            let options = _libraryAllData;
            if (!validate.isRoomView()) {
                 
                let radioValue = $(DE.libFilterOptions + ":checked").val();
                let modelType = $(DE.selectModelType).val();
                //let fmsType = _equipmentTypeData.filter(x=>x.key.toLowerCase() == 'fms');
                //let keyType = current_parent.entityType.toUpperCase() == 'UNIT' || current_parent.entityType.toUpperCase() == 'FLOOR' ? 'htb' : 'fms';
                //TODO:MAKE GENERIC TYPE

                let fmsType = _equipmentTypeData.filter(x=>x.is_middleware_model_type == true);
                if (modelType != null) {
                    fmsType = _equipmentTypeData.filter(x=>x.key.toLowerCase() == modelType.toLowerCase());
                }
                //let fmsType = _equipmentTypeData.filter(x=>x.key.toLowerCase() == 'htb');
                if (radioValue) {
                    //x.model_type_id != fmsType[0].id
                    if (fmsType.length > 0) {
                        options = _rackLibraryData.filter(x=> x.model_id == radioValue && fmsType.filter(function (val) {
                            if (val.id == x.model_type_id) { return true; }
                        }).length == 0);
                    }
                    else { options = _rackLibraryData.filter(x=> x.model_id == radioValue); }
                }
                if (radioValue && radioValue == DE.FMSMap) {
                    //let fmsType = _equipmentTypeData.filter(x=>x.key.toLowerCase() == 'fms');
                    if (fmsType && fmsType.length)
                        options = _rackLibraryData.filter(x=> x.model_id == DE.equipmentKey && x.model_type_id && fmsType.filter(function (val) {
                            if (val.id == x.model_type_id) { return true; }
                        }).length > 0);
                }

            } else {
                options = _libraryAllData;
            }
            return options;
        },
        modelPositionX: function (d) {
            //return d.position.x ;
            return d.position.x - (d.width / 2);
        },
        modelPositionY: function (d) {
            //return d.position.y;
            return d.position.y - (d.height / 2);
        },
        renderOperation: function (id) {
            let index = _workSpaceActions.getIndex(id);
            if (index == -1)
                return '';

            return generate.renderOperation(_workAreaData[index].parent) + generate.transformText(_workAreaData[index]);
        },
        transformText: function (d) {
            return ' translate(' + (generate.modelPositionX(d)) + ',' + (generate.modelPositionY(d)) + ') rotate(' + d.rotation_angle + "," + d.width / 2 + "," + d.height / 2 + ")";
        },
        inBoundaryPos: function (parentId, model, position) {
            //console.log('position',position);
            let rect = _workSpaceActions.getAbsoluteRect(parentId);
            let parent = _workSpaceActions.select(parentId);
            let border_width = parent.border_width || 0;


            //if (border_width) {
            //    rect.width -= border_width;
            //    rect.height -= border_width;
            //}
            let param = {
                x: position.x + rect.x - (model.width / 2) + ((position.x > (rect.width / 2)) ? model.width + border_width : -border_width),
                y: position.y + rect.y - (model.height / 2) + ((position.y > (rect.height / 2)) ? model.height + border_width : -border_width)
            };
            switch (model.rotation_angle) {
                case 90:
                case 270:
                    param = {
                        x: position.x + rect.x - (model.height / 2) + ((position.x > (rect.height / 2)) ? model.height + border_width : -border_width),
                        y: position.y + rect.y - (model.width / 2) + ((position.y > (rect.width / 2)) ? model.width + border_width : -border_width)
                    };
                    break;
            }
            //console.log(param);
            return param;
        },
        positionShift: function () {
            let shiftX = 0, shiftY = 0;
            if (_keyPressed['37']) {
                shiftX = -1;
                shiftY = 0;
            }
            if (_keyPressed['38']) {
                shiftX = 0;
                shiftY = -1;
            }
            if (_keyPressed['39']) {
                shiftX = 1;
                shiftY = 0;
            }
            if (_keyPressed['40']) {
                shiftX = 0;
                shiftY = 1;
            }
            return { x: shiftX, y: shiftY };
        },
        linePath: function (_source, _target) {
            _workAreaData = _workSpaceActions.getWorkArea();
            let _rackData = _workAreaData[0];
            const curve = d3.line().curve(d3.curveStepAfter);
            let points = [];
            let sourceRect = _workSpaceActions.getAbsoluteRect(_source.id);
            let targetRect = _workSpaceActions.getAbsoluteRect(_target.id);
            sourceRect.x = (sourceRect.x - generate.modelPositionX(_rackData)) + sourceRect.width / 2;
            sourceRect.y = (sourceRect.y - generate.modelPositionY(_rackData)) + sourceRect.height / 2;
            targetRect.x = (targetRect.x - generate.modelPositionX(_rackData)) + targetRect.width / 2;
            targetRect.y = (targetRect.y - generate.modelPositionY(_rackData)) + targetRect.height / 2;
            points.push([sourceRect.x, sourceRect.y]);
            if (targetRect.y == sourceRect.y) {
                points.push([sourceRect.x, (sourceRect.y - sourceRect.height / 2) + 40]);
                //points = [];
                //points.push([sourceRect.x, sourceRect.y - sourceRect.height / 2]);
                //points.push([sourceRect.x, (sourceRect.y - sourceRect.height / 2) + 40]);
                //points.push([targetRect.x, (targetRect.y - targetRect.height / 2) + 40]);
                //points.push([targetRect.x, targetRect.y - targetRect.height / 2]);
            } else if (targetRect.y < sourceRect.y) {
                points.push([sourceRect.x, (sourceRect.y - sourceRect.height / 2) - 40]);
                //points = [];
                //points.push([sourceRect.x, sourceRect.y - sourceRect.height / 2]);
                //points.push([sourceRect.x, (sourceRect.y - sourceRect.height / 2) - 40]);
                //points.push([targetRect.x, targetRect.y - targetRect.height / 2]);
            } else if (targetRect.y > sourceRect.y) {
                points.push([sourceRect.x, sourceRect.y + 40]);
            }
            points.push([targetRect.x, targetRect.y]);
            return curve(points);
        },
        outLinePath: function () {
            _workAreaData = _workSpaceActions.getWorkArea();
            const curve = d3.line().curve(d3.curveStepAfter);
            let _rackData = _workAreaData[0];
            let points = [];
            let sourceRect = _workSpaceActions.getAbsoluteRect(_selectedModel.id);
            sourceRect.x = (sourceRect.x - generate.modelPositionX(_rackData)) + sourceRect.width / 2;
            sourceRect.y = (sourceRect.y - generate.modelPositionY(_rackData)) + sourceRect.height / 2;
            points.push([sourceRect.x, sourceRect.y]);
            points.push([sourceRect.x, sourceRect.y + 40]);
            points.push([(0 - _rackData.outer_border_width - 30), sourceRect.y + 40]);
            return curve(points);
        },
        outConnectionTarget: function () {
            let _position = { x: 0, y: 0 };
            let _rackData = _workAreaData[0];
            let sourceRect = _workSpaceActions.getAbsoluteRect(_selectedModel.id);
            sourceRect.y = (sourceRect.y - generate.modelPositionY(_rackData)) + sourceRect.height / 2;
            _position.x = (0 - _rackData.outer_border_width - 30);
            _position.y = sourceRect.y + 40;
            return _position;
        }
    };

    var render = {
        leftPanel: function (flag) {
            let $roomLeftPanel = $(DE.roomLeftPanel);
            let $roomRackInfo = $(DE.roomRackInfo);
            //let $ispLeftInfoPanel = $(DE.entityInfoPanel);

            //render.resetInfoContext();
            if (flag) {
                //room view 
                $roomLeftPanel.show();
                $roomLeftPanel.addClass(DE.clslibraryEntity);

                render.rackViewFooter(!validate.isRoomView());
                if (validate.isNavRoomLib()) {
                    render.infoToLeftPnl();
                }
                else {
                    render.leftPnlToInfo();
                }
            }
            else {
                //ISP view
                $roomLeftPanel.hide();
                $roomLeftPanel.removeClass(DE.clslibraryEntity);

                render.rackViewFooter(false);
                $roomRackInfo.hide();
            }
        },
        clearEntityInfo: function () {
            $(DE.roomRackInfo).html('');
            $(DE.roomRackInfo).html('<div class="NoRecordInfo" style="margin-left: 18px !important;text-align: center !important;">' + MultilingualKey.SI_ISP_GBL_NET_FRM_043 + '</div>');
            
        },
        roomView: function (flag) {
            let $tabRoomView = $(DE.tabRoomView);
            let $roomView = $(DE.roomView);
            let $conFilter = $(DE.connectionFilter);
            //let $tabispAllPanel = $(DE.ispAllPanel);
            if (flag) {
                $tabRoomView.show();
                $roomView.show();
                render.setRoomTab();
                //$(DE.leftPanel).hide();
            } else {
                $tabRoomView.hide();
                $roomView.hide();
                $conFilter.hide();
                render.setISPTab();
            }
            render.rackView(!flag);
        },
        model: function (svgMain) {

            svgMain.attr("pointer-events", function (d) {
                let ev = "visible";
                if (d.model_id != DE.portKey && !d.is_editable && d.is_static) { ev = "none"; }
                return ev;
            })
                        .attr("id", function (d) { return DE.modelElementId + d.id; })
                        .attr(DE.elementId, function (d) { return d.db_id; })
                 .attr(DE.portNumber, function (d) { return d.port_number; })
                  .attr(DE.superParent, function (d) { return d.super_parent; })
                 .attr(DE.portStatus, function (d) { return d.port_status_id; })
                 .attr(DE.networkId, function (d) { return d.network_id; })
                 .attr(DE.elementId, function (d) { return d.db_id; })
                         .attr(DE.dataModelId, function (d) {
                             return d.model_id;
                         }).attr(DE.dataModelTypeId, function (d) {
                             return d.model_type_id;
                         })
                        .attr(DE.libraryId, function (d) {
                            return d.lib_id;
                        })
                         .attr(DE.dataUnitsNo, function (d) {
                             return d.no_of_units;
                         })
                        .attr(DE.libraryImgId, function (d) { return d.img_id; })
                        .attr("class", function (d) { return DE.modelClass })
                        //.attr("x", function (d) { return d.position.x; })
                        //.attr("y", function (d) { return d.position.y; })

                        .attr("height", function (d) {
                            let height = d.height;
                            //if (d.rotation_angle == 90 || d.rotation_angle == 270) {
                            //    height = d.height + d.width;
                            //}
                            return height;
                        })
                        .attr("width", function (d) {
                            let width = d.width;
                            //if (d.rotation_angle == 90 || d.rotation_angle == 270) {
                            //    width = d.height + d.width;
                            //}
                            return width;
                        })


                        //.call(d3.drag()
                        //.on("start", event.onStartModelDrag)
                        //.on("drag", event.onModelDrag)
                        //.on("end", event.onEndModelDrag))
                        //.on("contextmenu", event.onModelContextMenu)
                        .on("dblclick", event.onModeldblClick)
            .on("click", event.onModelClick).on('mouseover', event.onModelMouseOver);
            // function (d) { return { super_parent: d.super_parent, port_number: d.port_number, short_network_id: d.short_network_id, name: d.name } },
            let content = svgMain;
            //let content = group.append("svg").attr("height", function (d) { return d.height; })
            //            .attr("width", function (d) { return d.width; });

            ////Add background rectangle
            let background = content.append("g");
            background.append("g").append("rect")
                 .attr("id", function (d) { return DE.elementRect + d.id; })
                 //.attr("class", DE.elementRect)
                 .attr("class", function (d) {
                     let cl = DE.elementRect;
                     //if (d.model_type != undefined && d.model_type.toUpperCase() == 'EQUIPMENT')
                     //{
                     //    cl += ' equipment';
                     //    if (validate.isRoomView())
                     //    {
                     //        cl += ' equipment roomViewEquipment';
                     //    }
                     //}
                        
                     if (d.model_id == DE.portKey)
                         cl += ' port';
                     //if (networkPortColor['C'] == d.port_status_color) 
                     //    cl += ' connected';

                     if (!validate.isRoomView()) {
                         if (d.model_id == DE.equipmentKey && !d.is_view_enabled)
                             cl += ' ' + DE.roleDisabledClass;
                     }
                     else if (!d.is_view_enabled)
                         cl += ' ' + DE.roleDisabledClass;
                     return cl;
                 })
            .attr("x", function (d) {
                let x = 0;

                return x;
            })
            .attr("y", function (d) {
                let y = 0;

                return y;
            })
            .attr("height", function (d) { return d.height; })
            .attr("width", function (d) { return d.width; })
            .style("fill", function (d) {
                //let color = d.model_id == DE.chessisKey ? DE.chassisColor : d.color;
                let color = d.color;
                    if (!color || color == null) { color = DE.color_code; }
                if (d.model_id == DE.portKey) {
                    color = DE.portColor;
                }
                if (d.model_id == DE.trayKey) {
                    color = DE.trayColor;
                }
                if (validate.isRoomView() && d.id > 1) {
                    color = networkRackColor[d.network_status];
                }
                return color;
            })
            .style("opacity", function (d) {
                //let o = 1;
                //if (d.model_id == DE.portKey) {
                //    o = DE.portOpacity;
                //}
                //return o;
            })
            .style("stroke", function (d) {
                let stroke = d.stroke;
                    if (!stroke || stroke == null) { stroke = DE.stroke; }
                //if (d.model_id == DE.chessisKey) {
                //    stroke = DE.chassisStroke;
                //}
                return stroke;
            })
            .style("stroke-width", function (d) {
                let sWidth = 3;
                    if (validate.isRoomView() && d.id == 1) { sWidth = 1; }
                return sWidth;

            }).append('title').text(function (d) { return networkPortColor['C'] == d.port_status_color ? "" : (d.port_status_id == 3 || d.port_status_id == 4 ? d.name + "(" + d.port_comment + ")" : d.name); });

            background.filter(function (d) {
                return d.model_id == DE.cardKey;
            }).append('rect')
                        .attr("height", function (d) { return d.height; })
                        .attr("width", function (d) { return d.width; })
                        .style("fill", function (d) {
                            let color = DE.cardHatch;

                            //if (d.model_id == DE.cardKey) {
                            //    color = DE.cardHatch;
                            //}
                            return color;
                        });
            ////Chassis inner container
            background.filter(function (d) {
                return d.model_id == DE.chessisKey;
            })
                .append("svg").attr("pointer-events", "none")
                .attr("x", function (d) { return d.border_width; })
                .attr("y", function (d) { return d.border_width; })
                .attr("height", function (d) { return Math.abs(d.height - (2 * d.border_width)); })
                .attr("width", function (d) { return Math.abs(d.width - (2 * d.border_width)); })
                .append("rect")
                //.attr("x", function (d) { return d.border_width; })
                //.attr("y", function (d) { return d.border_width; })
                .attr("x", function (d) { return 0; })
                .attr("y", function (d) { return 0; })
                .attr("height", function (d) { return Math.abs(d.height - (2 * d.border_width)); })
                .attr("width", function (d) { return Math.abs(d.width - (2 * d.border_width)); })
              .style("fill", function (d) {
                  return DE.chassisInnerColor;//d.color;
              })
              .style("stroke", function (d) {
                  let stroke = d.stroke;
                    if (!stroke || stroke == null) { stroke = DE.chassisStroke; }
                  return stroke;
              })
              .style("stroke-width", function (d) { return '3'; }).append('title').text(function (d) { return d.name; });

            //Add label data
            let labelArea = content.append("g");
            labelArea.filter(function (d) {
                return d.model_id == DE.labelKey;
            }).append("image")
            .attr('href', function (d) { return d.bg_image; })
            .attr('height', function (d) { return d.height; })
            .attr('width', function (d) { return d.width; });

            labelArea.filter(function (d) {
                return d.model_id && d.model_id != DE.portKey && d.model_id != DE.trayKey;
            }).append("text")
                    .attr("pointer-events", "none")
                    .attr("x", function (d) { return d.width / 2; })
                    .attr("y", function (d) {
                        let y = d.height / 2;
                        if (d.model_id == DE.chessisKey) {
                            y = 16;
                        }
                        if (d.model_id == DE.labelKey) {
                            y += d.font_size / 4;
                        }
                        return y;
                    })
                    .style('fill', function (d) {
                        let color = '#000000';
                        if (d.model_id == DE.labelKey) {
                            color = d.font_color;
                        }
                        return color;
                    })
                    .style('font-size', function (d) {
                        let size = DE.defaultFontSize;
                        if (d.model_id == DE.labelKey) {
                            size = d.font_size;
                        }
                        return size + 'px';
                    })
                    .style("text-anchor", "middle")
                    .style('writing-mode', function (d) {
                        let mode = '';
                        if (d.text_orientation)
                            mode = d.text_orientation;
                        return mode;
                    })
                    .text(function (d) {
                        if (d.model_id == DE.labelKey || d.model_id == DE.chessisKey) {
                            return d.name;
                        }
                    });

            ////Add image svg data 
            let imgContainer = content.append("g");


            //Filter by image data and id; add viewBox
            imgContainer
                .filter(function (d) {
                    return !isNaN(d.img_id) && d.image_data && d.image_data != '';
                })
                .append("svg")
                .attr("pointer-events", "none")
                .attr("preserveAspectRatio", "xMinYMin meet")

                .attr("viewBox", function (d) {
                    let v = "0 0 50 50";
                    if (validate.isRoomView()) { v = "0 0 40 24"; }
                    return v;
                })
                .attr("height", function (d) {
                    let h = d.height * 0.9;
                    h = h > 0 ? h : 0;
                    if (validate.isRoomView()) {
                        h = d.height * 0.5;
                        if (d.model_type != undefined && d.model_type.toUpperCase() == 'EQUIPMENT') {
                            h = d.height * 0.9;
                        }
                    }
                    return h;
                })
                .attr("width", function (d) {
                    let w = d.width * 0.9;
                    w = w > 0 ? w : 0;
                    if (validate.isRoomView()) {
                        w = d.width * 0.5;

                        if (d.model_type != undefined && d.model_type.toUpperCase() == 'EQUIPMENT') {
                            w = d.width * 0.9;
                        }
                       
                    }
                    console.log("d.width :"+d.width + "//width: " + w);
                    return w;
                })
                 .attr("x", function (d) {
                     let x = d.width * 0.05;
                     if (validate.isRoomView()) {
                         let min = (d.width * 0.5);
                         x = d.width / 2 - min / 2;
                     }
                     return x;

                 })
                .attr("y", function (d) {
                    let y = d.height * 0.05;
                    if (validate.isRoomView()) {
                        let min = (d.height * 0.5);
                        y = d.height / 2 - min / 2;
                        if (d.model_type != undefined && d.model_type.toUpperCase() == 'EQUIPMENT') {
                            if (d.model_id == 0) {
                                y = 0;
                            } else {
                                y = d.height / 2 - min / 2; 
                            }
                           
                        }
                    }
                    return y;
                })

                .html(function (d) {
                    let innerData = d.image_data;
                    if (validate.isRoomView()) {
                        innerData = innerData.replaceAll('[COLOR]',function(){
                            var color = networkRackColor[d.network_status];
                            if (d.model_id == DE.equipmentKey) {
                                color = "white";
                            }
                            return color;
                        } )
                    }
                    else {
                        if (d.model_id == DE.portKey) {
                            innerData = innerData.replaceAll('[BORDER_COLOR]', (d.border_color == undefined ? DE.defaultBorderColor : d.border_color))
                        }

                        if (d.model_id == DE.portKey) {

                            innerData = innerData.replaceAll('[STATUS_COLOR]', d.port_status_color);
                            if (networkPortColor['C'] == d.port_status_color) {

                                innerData = innerData.replaceAll('[BLINK_CLASS]', DE.blinkClass);
                            } else {
                                innerData = innerData.replaceAll('[STATUS_COLOR]', networkPortColor['V']);
                            }

                        }

                        //if (_connectionChecked) {
                        //    innerData = innerData.replaceAll('[STATUS_COLOR]', d.port_status_color);
                        //    if (networkPortColor['C'] == d.port_status_color) {
                        //        innerData = innerData.replaceAll('[BLINK_CLASS]', DE.blinkClass);
                        //    }
                        //}
                        //else {
                        //    if (d.model_id == DE.portKey) { innerData = innerData.replaceAll('[STATUS_COLOR]', networkPortColor['V']); }
                        //}
                    }
                    return innerData;
                });

        },
        allModels: function () {


            render.nodeTree();
            if (_workAreaData.length) {
                const workData = _workAreaData[0];
                //Ruler rendering
                render.ruler(workData);

                //Door rendering
                if (validate.isRoomView()) {
                    render.roomBorder(workData);
                    if (DE.defaultDoor && DE.defaultDoorType !== 'none')
                        render.roomDoor({ position: DE.defaultDoor, width: generate.meterToPixel(DE.defaultDoorWidth), type: DE.defaultDoorType });
                }
                if (!validate.isRoomView()) {
                    render.rackBorder(workData);
                    render.allEquipmentStatus();
                }
            }
            render.modelFocus(_selectedModel.id);
        },
        modelChildren: function (root, nodes) {
            if (!nodes || (nodes && nodes.length == 0)) {
                return null;
            }
            else {
                let parentData = root.selectAll('.' + DE.modelClass)
                                      .data(nodes)
                                      .enter();
                let parent = parentData.append("g").attr('id', function (d) {
                    return DE.modelGroup + d.id;
                }).attr("transform", function (d) {
                    let x = d.position.x;
                    let y = d.position.y;
                 

                    return 'translate(' + generate.modelPositionX(d) + ',' + generate.modelPositionY(d) + ") rotate(" + d.rotation_angle + "," + d.width / 2 + "," + d.height / 2 + ")";
                }).call(d3.drag()
                       .on("start", event.onStartModelDrag)
                       .on("drag", event.onModelDrag)
                       .on("end", event.onEndModelDrag))
                       .on("contextmenu", event.onModelContextMenu);

                parent = parent.append("svg");
                render.model(parent);

                parent.each(function (d) {
                    render.modelChildren(d3.select(this), d.children);
                });

            }
        },
        nodeTree: function () {
            _workAreaData = _workSpaceActions.getWorkArea();
            // _$workArea.selectAll(DE.model).remove();
            _$workArea.selectAll('*').remove();
            if (_workAreaData.length) {
                _nodeTree = [];
                _nodeTree.push(_workAreaData[0]);
                generate.modelTree(_workAreaData[0].id, _workAreaData, _workAreaData[0]);
                render.modelChildren(_$workArea, _nodeTree);
            }
        },
        horizontalRuler: function (e) {
            d3.select("#" + DE.horizontalRuler).remove();
            let container = _$workArea.append("svg")
                    .attr("pointer-events", "none")
                    .attr("id", DE.horizontalRuler)
                    .attr("x", generate.modelPositionX(e))
                    .attr("y", generate.modelPositionY(e) - 50)
                    .attr("height", 30)
                    .attr("width", e.width);
            container.append("rect")
                .attr("pointer-events", "none")
                    .attr("id", "h-ruler-bg")
                    .attr("x", 0)
                    .attr("y", 0)
                    .attr("height", 30)
                    .attr("width", e.width)
                    .style('fill', '#ffffff')
                    .style("fill-opacity", 1)
                    .style("stroke", "#000").style("stroke-width", "0.4");
            let rulerData = generate.rulerData(e.width, _scaleFactor.gridCellSize / 5);
            let labels = container.selectAll('.h-scaler').data(rulerData).enter();
            labels.append("rect")
                .attr("class", "h-scaler")

                .attr("x", function (d, i) {
                    let h = d + 1;
                    if (i % 5 == 0) {
                        h = d;
                    }
                    return h;
                })
                .attr("y", function (d, i) {
                    let h = 20;
                    if (i % 5 == 0) {
                        h = 0;
                    }
                    return h;
                })
                .attr("height", function (d, i) {
                    let h = 10;
                    if (i % 5 == 0) {
                        h = 30;
                    }
                    return h;
                })
                .attr("width", 0.5)
                 .style('fill', '#000')
                 .style("fill-opacity", 1)
                 .style("stroke", "#000").style("stroke-width", "0.1");
            labels.filter(function (d, i) {
                return i % 5 == 0;
            }).append("text")
                 .style("font-size", '9px')
                 .attr("x", function (d, i) {
                     let val = '' + generate.pixelToMeter(i * _scaleFactor.gridCellSize);
                     if (!validate.isRoomView()) {
                         val = '' + generate.pixelToMM(i * _scaleFactor.gridCellSize);
                     }
                     return d - (val.length * 4) - 10;
                 })
                 .attr("y", function (d) { return 15; })
             .text(function (d, i) {
                 let val = generate.pixelToMeter(i * _scaleFactor.gridCellSize);
                 if (!validate.isRoomView()) {
                     val = generate.pixelToMM(i * _scaleFactor.gridCellSize);
                 }
                 return val;
             })
        },
        resetHorizontalRuler: function () {
            d3.select("#" + DE.horizontalRuler).attr("x", 0)
                                 .attr("y", 0)
                                 .attr("height", 0)
                                 .attr("width", 0)
                                  .html('');
        },
        verticalRuler: function (e) {


            d3.select("#" + DE.verticalRuler).remove();
            let container = _$workArea.append("svg")
                    .attr("pointer-events", "none")
                    .attr("id", DE.verticalRuler)
                    .attr("x", generate.modelPositionX(e) - 50)
                    .attr("y", generate.modelPositionY(e))
                    .attr("height", e.height)
                    .attr("width", 30);
            container.append("rect")
                .attr("pointer-events", "none")
                    .attr("id", "v-ruler-bg")
                    .attr("x", 0)
                    .attr("y", 0)
                    .attr("height", e.height)
                    .attr("width", 30)
                    .style('fill', '#ffffff')
                    .style("fill-opacity", 1)
                    .style("stroke", "#000")
                    .style("stroke-width", "0.4");
            let rulerData = generate.rulerData(e.height, _scaleFactor.gridCellSize / 5);
            let labels = container.selectAll('.v-scaler').data(rulerData).enter();
            labels.append("rect")
                .attr("class", "v-scaler")

                .attr("y", function (d, i) {
                    let h = d + 1;
                    if (i % 5 == 0) {
                        h = d;
                    }
                    return h;

                })
                .attr("x", function (d, i) {
                    let h = 20;
                    if (i % 5 == 0) {
                        h = 0;
                    }
                    return h;
                })
                .attr("width", function (d, i) {
                    let h = 10;
                    if (i % 5 == 0) {
                        h = 30;
                    }
                    return h;
                })
                .attr("height", 0.5)
                .style('fill', '#000')
                .style("fill-opacity", 1)
                .style("stroke", "#000").style("stroke-width", "0.1");;
            labels.filter(function (d, i) {
                return i % 5 == 0;
            }).append("text")
                .style("font-size", '9px')
                .attr("x", 2)
                .attr("y", function (d) { return d - 2; })
             .text(function (d, i) { return generate.pixelToMeter(i * _scaleFactor.gridCellSize); });

        },
        resetVerticalRuler: function () {
            d3.select("#" + DE.verticalRuler).attr("x", 0)
                                 .attr("y", 0)
                                 .attr("height", 0)
                                 .attr("width", 0)
                                  .html('');
        },
        ruler: function (e) {
            if (validate.isRoomView()) { render.verticalRuler(e); }
            render.horizontalRuler(e);
        },
        resetRuler: function (e) {
            render.resetVerticalRuler();
            render.resetHorizontalRuler();
        },
        modelOperation: function (d) {

            action.addModel({
                id: 0
            });

            _workAreaData = _workSpaceActions.getWorkArea();
            _workAreaData[0].position.x += 50 + _workAreaData[0].width / 2;
            _workAreaData[0].position.y += 50 + _workAreaData[0].height / 2;
            render.allModels();
            render.ruler(_workAreaData[0]);



        },
        setInCenter: function (model, parent) {
            let rect = parent || _$workArea.node().getBoundingClientRect()
            let $grid = generate.currentWorkAreaContext();
            let pos_x = rect.width / 2 - model.width / 2, pos_y = rect.height / 2 - model.height / 2;

            model.position.x = pos_x < 0 ? 0 : pos_x;
            model.position.y = pos_y < 0 ? 0 : pos_y;
        },
        setInTop: function (model, parent) {
            let rect = parent || _$workArea.node().getBoundingClientRect()
            let pos_x = rect.width / 2 - model.width / 2, pos_y = 50;
            model.position.x += (pos_x < 0 ? 0 : pos_x);
            model.position.y += (pos_y < 0 ? 0 : pos_y);
        },
        modelFocus: function (id, isReset) {
            isReset = isReset !== undefined ? isReset : true;
            if (isReset) {
                d3.selectAll('.' + DE.elementRect).classed(DE.selectedClass, false);
            }
            if (id)
                d3.select('#' + DE.elementRect + id).classed(DE.selectedClass, true);
        },
        setWorkSpaceSize: function (height, width) {
            let $grid = generate.currentWorkAreaContext();
            let minHeight = $grid.height();
            let minWidth = $grid.width();
            if (height < minHeight) {
                height = minHeight;
            }
            if (width < minWidth) {
                width = minWidth;
            }
            _$workArea.style('height', height).style('width', width-5);
        },
        setRoomTab: function () {
             
            $(DE.tabRoomView).addClass('active').addClass('show');
            $(DE.tabIspView).removeClass('active').removeClass('show');
            $(DE.roomPanel).addClass('active').addClass('show');
            $(DE.ispPanel).removeClass('active').removeClass('show');
        },
        setISPTab: function () {
            $(DE.tabIspView).addClass('active').addClass('show');
            $(DE.tabRoomView).removeClass('active').removeClass('show');
            $(DE.ispPanel).addClass('active').addClass('show');
            $(DE.roomPanel).removeClass('active').removeClass('show');
        },
        setEquipmentViewTab: function () {
             
            $(DE.tabEquipmentView).addClass('active').addClass('show');
            $(DE.tabRoomView).removeClass('active').removeClass('show');
        },
        library: function () {
            let libCount = _libraryData.length;
            _$libArea.attr('height', Math.ceil(libCount / 2) * DE.librarySize);
            _$libArea.selectAll("*").remove();
            let libData = _$libArea.selectAll(DE.btnLibAdd)
                .data(_libraryData);
            libData.exit().remove();
            let libMain = libData.enter()
                 .append("svg")
                 .attr("pointer-events", "none")
                 .attr("class", function (d) { return DE.libAddButtonClass; })
                 .attr("x", function (d) {
                     let x = d.position.x + (d.width * 0.22);
                     if (d.model_id == DE.trayKey) {
                         x = d.position.x + (d.width * 0.17);
                     } else if(d.model_id == DE.equipmentKey) {
                         x = d.position.x + (d.width * 0.14);
                     }
                     return x;
                 })
                 .attr("y", function (d) {
                     let y = d.position.y + 10;
                     if (d.model_id == DE.trayKey) {
                         y = d.position.y + 5;
                     }
                     return y;
                 })
                 .attr("height", function (d) {
                     var h = d.height * 0.7;
                     if (d.model_id == DE.equipmentKey) {
                         h = d.height;
                     }
                     return h;
                 })
                 .attr("width", function (d) {
                     var w = d.width * 0.7;
                     if (d.model_id == DE.equipmentKey) {
                         w = d.width;
                     }
                     return w;                     
                 })

                 .html(function (d) {
                     let imgData = d.image_data;
                     if (imgData)
                         imgData = imgData.replaceAll('[COLOR]', networkRackColor['none']);
                     return imgData;
                 });

            //Main library button
            libData.enter().append("g").append("rect")
                            .attr("x", function (d) { return d.position.x; })
                             .attr("pointer-events", function (d) {
                                 let show = "visible";
                    if (d.enabled) { show = "visible"; }
                                 else { show = "none"; }
                                 return show;
                             })

                            .attr("y", function (d) { return d.position.y; })
                            .attr("height", function (d) { return d.height; })
                            .attr("width", function (d) { return d.width; })
                            .style("fill", function (d) {
                                let color = "transparent";
                    if (d.enabled) { color = "transparent"; }
                                else { color = "#c1bdbd"; }
                                return color;
                            })
                             .style("fill-opacity", function (d) {
                                 let o = 1;
                    if (d.enabled) { o = 1; }
                                 else { o = 0.7; }
                                 return o;
                             })
                            .style("stroke", function (d) { return "#a7a4a7"; })
                            .style("stroke-width", function (d) { return '1'; })
                            .attr(DE.libraryId, function (d) {
                                return d.db_id;
                            }).attr(DE.libraryImgId, function (d) {
                                return d.img_id;
                            })
                            .attr(DE.dataModelId, function (d) {
                                return d.model_id;
                            }).attr(DE.dataModelTypeId, function (d) {
                                return d.model_type_id;
                            })
                            .attr(DE.dataColorCode, function (d) {
                                return d.db_color;
                            }).attr(DE.dataStrokeCode, function (d) {
                                return d.stroke;
                            }).attr(DE.dataUnitsNo, function (d) {
                                return d.no_of_units;
                            })
                            .attr("data-height", function (d) { return d.db_height; })
                            .attr("data-width", function (d) { return d.db_width; })
                            .attr("data-depth", function (d) { return d.db_depth; })
                            .attr("data-border-width", function (d) { return d.db_border_width; })
                            .attr("data-name", function (d) { return d.name; })
                            .attr("data-border-color", function (d) { return d.border_color; })
                            .attr("class", function (d) { return DE.libAddButtonClass; })
                            .html(function (d) { return d.image_data; })
                            .on('click', event.onLibAddButton)
                            .append('title').text(function (d) { return d.name; });

            libMain.filter(function (d) {
                return d.image_data && d.image_data != '';
            }).attr("preserveAspectRatio", "xMinYMin meet")
                .attr("viewBox", function (d) {
                    var viewBox = "0 0 50 50";
                    if (d.model_id == DE.equipmentKey) {
                        viewBox = "0 0 31 31";
                    }
                    return viewBox;
                });
            //Library display rect
            libMain.filter(function (d) {
                return d.image_data == '' || !d.image_data;
            }).append("g").append("rect")
                        .attr("pointer-events", "none")
                        .attr("x", function (d) {
                            let x = 5;
                            if (d.model_id == DE.trayKey) {
                                x = -5;
                            }
                            return x;
                        })
                        .attr("y", function (d) {
                            let h = 5;
                            if (d.model_id == DE.trayKey) {
                                h = 25;
                            }
                            return h;
                        })
                        .attr("height", function (d) {
                            let h = 70;
                            if (d.model_id == DE.trayKey) {
                                h = 1;
                            }
                            return h;
                        })
                        .attr("width", function (d) {
                            let w = 70;
                            if (d.model_id == DE.trayKey) {
                                w = (d.width * 0.7) + 10;
                            }
                            return w;
                        })
                        .style("fill", function (d) {
                            let res = d.color;
                            if (d.model_id == DE.trayKey) {
                                res = 'transparent';
                            }
                            if (d.model_id == DE.chessisKey) {
                                res = DE.chassisInnerColor;
                            }
                            return res;
                        })
                        .style("stroke", function (d) {
                            let res = d.stroke;
                            if (d.model_id == DE.trayKey) {
                                res = DE.portColor;
                            }
                            if (d.model_id == DE.chessisKey) {
                                res = DE.chassisColor;
                            }
                            return res;
                        })
                        .style("stroke-width", function (d) { return '3'; });

            //Dimension and label part
            let dimension = libData.enter().append("g");
            dimension.append("rect")
                    .attr("pointer-events", "none")
                    .attr("x", function (d) { return d.position.x + d.width / 4 - 25; })
                    .attr("y", function (d) { return d.position.y + d.height - 40; })
                    .attr("height", function (d) { return 20; })
                    .attr("width", function (d) { return (d.width); })
                    .style('fill', DE.portColor);
            dimension.append("text")
                    .attr("pointer-events", "none")
                    .attr("x", function (d) { return d.position.x + d.width / 2; })
                    .attr("y", function (d) { return d.position.y + d.height - 25; })
                    .style('fill', '#FFFFFF')
                    .style("text-anchor", "middle")
                    .text(function (d) {
                        let h = d.db_height, w = d.db_width;
                        if (!validate.isRoomView()) {
                            //h = generate.pixelToMeter(h);
                            //w = generate.pixelToMeter(w);
                        }
                        return "" + h + " X " + w;
                    });


            libData.enter().append("g").append("text")
                            .attr("pointer-events", "none")
                            .attr("x", function (d) { return d.position.x + d.width / 2; })
                            .attr("y", function (d) { return d.position.y + d.height - 5; })
                            .style("text-anchor", "middle")
                            .html(function (d) {
                                let name = d.name;
                                let port = '';
                                if (d.no_of_port) { port = '(' + d.no_of_port + ')'; }
                                if (name && (name.length + port.length) > 12) {
                                    name = name.substring(0, 9) + '...';
                                }
                    if (d.model_id == DE.equipmentKey) { name = name + port; }

                                return name;
                            });
        },
        libraryRoomView: function () {
            let libCount = _libraryData.length;
            _$libRoomViewArea.attr('height', Math.ceil(libCount / 2) * DE.librarySize);
            _$libRoomViewArea.selectAll("*").remove();
            let libData = _$libRoomViewArea.selectAll(DE.btnLibAdd)
                .data(_libraryData);
            libData.exit().remove();
            let libMain = libData.enter()
                 .append("svg")
                 .attr("pointer-events", "none")
                 .attr("class", function (d) { return DE.libAddButtonClass; })
                 .attr("x", function (d) {
                     let x = d.position.x + (d.width * 0.14);
                     if (d.model_id == DE.trayKey) {
                         x = d.position.x + (d.width * 0.17);
                     }
                     //console.log("X Location:" + x);
                     return x;
                 })
                 .attr("y", function (d) {
                     let y = d.position.y + 10;
                     if (d.model_id == DE.trayKey) {
                         y = d.position.y + 5;
                     }
                     //console.log("Y Location:" + y);
                     return y;
                 })
                 .attr("height", function (d) {  return d.height; })
                 .attr("width", function (d) { return (d.width); })

                 .html(function (d) {
                     let imgData = d.image_data;
                     if (imgData)
                         imgData = imgData.replaceAll('[COLOR]', networkRackColor['none']);
                     return imgData;
                 });

            //Main library button
            libData.enter().append("g").append("rect")
                            .attr("x", function (d) { return d.position.x; })
                             .attr("pointer-events", function (d) {
                                 let show = "visible";
                    if (d.enabled) { show = "visible"; }
                                 else { show = "none"; }
                                 return show;
                             })

                            .attr("y", function (d) { return d.position.y; })
                            .attr("height", function (d) { return d.height; })
                            .attr("width", function (d) { return d.width; })
                            .style("fill", function (d) {
                                let color = "transparent";
                    if (d.enabled) { color = "transparent"; }
                                else { color = "#c1bdbd"; }
                                return color;
                            })
                             .style("fill-opacity", function (d) {
                                 let o = 1;
                    if (d.enabled) { o = 1; }
                                 else { o = 0.7; }
                                 return o;
                             })
                            .style("stroke", function (d) { return "#a7a4a7"; })
                            .style("stroke-width", function (d) { return '1'; })
                            .attr(DE.libraryId, function (d) {
                                return d.db_id;
                            }).attr(DE.libraryImgId, function (d) {
                                return d.img_id;
                            })
                            .attr(DE.dataModelId, function (d) {
                                return d.model_id;
                            })
                            .attr(DE.dataModelTypeId, function (d) {
                                return d.model_type_id;
                            })
                            .attr("data-model-type", function (d) {
                                return 'Equipment';
                            })
                            .attr(DE.dataColorCode, function (d) {
                                return d.db_color;
                            }).attr(DE.dataStrokeCode, function (d) {
                                return d.stroke;
                            }).attr(DE.dataUnitsNo, function (d) {
                                return d.no_of_units;
                            })
                            .attr("data-height", function (d) { return d.db_height; })
                            .attr("data-width", function (d) { return d.db_width; })
                            .attr("data-depth", function (d) { return d.db_depth; })
                            .attr("data-border-width", function (d) { return d.db_border_width; })
                            .attr("data-name", function (d) { return d.name; })
                            .attr("data-border-color", function (d) { return d.border_color; })
                            .attr("class", function (d) { return DE.libAddButtonClass; })
                            .html(function (d) { return d.image_data; })
                            .on('click', event.onLibAddEquipmentButton)
                            .append('title').text(function (d) { return d.name; });

            libMain.filter(function (d) {
                return d.image_data && d.image_data != '';
            }).attr("preserveAspectRatio", "xMinYMin meet")
                .attr("viewBox", function (d) { return "0 0 31 31"; });
            //Library display rect
            libMain.filter(function (d) {
                return d.image_data == '' || !d.image_data;
            }).append("g").append("rect")
                        .attr("pointer-events", "none")
                        .attr("x", function (d) {
                            let x = 5;
                            if (d.model_id == DE.trayKey) {
                                x = -5;
                            }
                            return x;
                        })
                        .attr("y", function (d) {
                            let h = 5;
                            if (d.model_id == DE.trayKey) {
                                h = 25;
                            }
                            return h;
                        })
                        .attr("height", function (d) {
                            let h = 70;
                            if (d.model_id == DE.trayKey) {
                                h = 1;
                            }
                            return h;
                        })
                        .attr("width", function (d) {
                            let w = 70;
                            if (d.model_id == DE.trayKey) {
                                w = (d.width * 0.7) + 10;
                            }
                            return w;
                        })
                        .style("fill", function (d) {
                            let res = d.color;
                            if (d.model_id == DE.trayKey) {
                                res = 'transparent';
                            }
                            if (d.model_id == DE.chessisKey) {
                                res = DE.chassisInnerColor;
                            }
                            return res;
                        })
                        .style("stroke", function (d) {
                            let res = d.stroke;
                            if (d.model_id == DE.trayKey) {
                                res = DE.portColor;
                            }
                            if (d.model_id == DE.chessisKey) {
                                res = DE.chassisColor;
                            }
                            return res;
                        })
                        .style("stroke-width", function (d) { return '3'; });

            //Dimension and label part
            let dimension = libData.enter().append("g");
            dimension.append("rect")
                    .attr("pointer-events", "none")
                    .attr("x", function (d) { return d.position.x + d.width / 4 - 25; })
                    .attr("y", function (d) { return d.position.y + d.height - 40; })
                    .attr("height", function (d) { return 20; })
                    .attr("width", function (d) { return (d.width); })
                    .style('fill', DE.portColor);
            dimension.append("text")
                    .attr("pointer-events", "none")
                    .attr("x", function (d) { return d.position.x + d.width / 2; })
                    .attr("y", function (d) { return d.position.y + d.height - 25; })
                    .style('fill', '#FFFFFF')
                    .style("text-anchor", "middle")
                    .text(function (d) {
                        let h = d.db_height, w = d.db_width;
                        if (!validate.isRoomView()) {
                            //h = generate.pixelToMeter(h);
                            //w = generate.pixelToMeter(w);
                        }
                        return "" + h + " X " + w;
                    });


            libData.enter().append("g").append("text")
                            .attr("pointer-events", "none")
                            .attr("x", function (d) { return d.position.x + d.width / 2; })
                            .attr("y", function (d) { return d.position.y + d.height - 5; })
                            .style("text-anchor", "middle")
                            .html(function (d) {
                                let name = d.name;
                                let port = '';
                                if (d.no_of_port) { port = '(' + d.no_of_port + ')'; }
                                if (name && (name.length + port.length) > 12) {
                                    name = name.substring(0, 9) + '...';
                                }
                    if (d.model_id == DE.equipmentKey) { name = name + port; }

                                return name;
                            });
        },
        filterRoomViewLibList: function (options) {
            let radioValue =  DE.equipmentKey;
            let searchText = $(DE.txtSearchRoomViewLibraryData).val();
            let libTypes = $(DE.selectRoomViewLibTypes + " option:selected").val();
            let typeText = $(DE.selectRoomViewLibTypes + " option:selected").text();
            let arr = [];
            let check = false;
          
                if (searchText) {
                    searchText = searchText.trim().toLowerCase();
                }
                if (options.length) {

                    for (var i = 0; i <= options.length - 1; i++) {
                        options[i].hidden = false;
                        check = false;
                        if (!check && (!searchText || options[i].model_name.toLowerCase().includes(searchText))) {
                            check = true;
                        }

                        if (check && validate.isRoomView()) {
                            //Rack view
                            if (check && ((libTypes == 'all') || ((radioValue != DE.equipmentKey) || (!libTypes || options[i].model_type_id == libTypes)
                            && (!options[i].model_type_master_name || options[i].model_type_master_name == typeText)))) {
                                check = true;
                                options[i].hidden = false;
                            }
                            else {
                                check = false;
                                options[i].hidden = true;
                               
                            }

                    
                        }
                    
                        if (searchText && check) {
                            options[i].enabled = check;
                            if (!options[i].hidden)
                                arr.push(options[i]);
                        }
                        else if (!searchText) {
                            options[i].enabled = check;
                            if (!options[i].hidden)
                                arr.push(options[i]);
                        }
                    }
                } //}
                $(DE.lblRoomViewMsg).html('');
                if (arr.length == 0) {
                   
                    $(DE.lblRoomViewMsg).html(MultilingualKey.SI_OSP_GBL_GBL_RPT_001);//No record found!
                }
                generate.libraryList(arr);
                render.libraryRoomView();
        },

        filterLibList: function (options) {
            let radioValue = $(DE.libFilterOptions + ":checked").val();
            let searchText = $(DE.txtSearchLibraryData).val();
            let libTypes = $(DE.selectLibTypes + " option:selected").val();
            let typeText = $(DE.selectLibTypes + " option:selected").text();
            let $fmsEntity = $(DE.selectExistedFms + " option:selected");
            let arr = [];
            let check = false;
            _workAreaData = _workSpaceActions.getWorkArea();
            let selectedModel = _workAreaData[0];
            if (selectedModel) {
                selectedModel.border_width = isNaN(selectedModel.border_width) ? 0 : selectedModel.border_width;
                if (searchText) {
                    searchText = searchText.trim().toLowerCase();
                }
                if (options.length) {

                    for (var i = 0; i <= options.length - 1; i++) {
                        options[i].hidden = false;
                        check = false;
                        if (!check && (!searchText || options[i].model_name.toLowerCase().includes(searchText))) {
                            check = true;
                        }

                        if (check && validate.isRoomView()) {
                            //Room view
                            //check = true;
                            //dimension check
                            if (check && ((selectedModel.height - selectedModel.border_width * 2) >= generate.meterToPixel(options[i].height)
                                && (selectedModel.width - selectedModel.border_width * 2) >= generate.meterToPixel(options[i].width))) {
                                check = true;
                            } else {  check = false; }
                        }
                        else {
                            //Rack view
                            if (check && ((libTypes == 'all') || ((radioValue != DE.equipmentKey) || (!libTypes || options[i].model_type_id == libTypes)
                            && (!options[i].model_type_master_name || options[i].model_type_master_name == typeText)))) {
                                check = true;
                            }
                            else {
                                check = false;

                                options[i].hidden = true;
                            }

                            //dimension check
                            if (check && ((selectedModel.height - selectedModel.border_width * 2) >= generate.meterToPixel(options[i].height)
                                && (selectedModel.width - selectedModel.border_width * 2) >= generate.meterToPixel(options[i].width))) {
                                check = true;
                            } else {  check = false; }

                            //Tray check
                            if (check && (options[i].model_id == DE.trayKey)) {
                                if ((selectedModel.height - selectedModel.border_width * 2) >= generate.mmToPixel(options[i].height)
                                && (selectedModel.width - selectedModel.border_width * 2) == generate.mmToPixel(options[i].width)) {
                                    check = true;
                                }
                                else {  check = false; }
                            }

                            ////Number of port and Vendor check
                            //// arrayList.includes(portNo.toString())
                            if (radioValue == DE.FMSMap) {
                                var optionsEntity = document.getElementById('selectExistedFms').options
                                var vendorsList = $.map(optionsEntity, function (option) {
                                    return $(option).attr("data-vendor-id");
                                });
                                var portsList = $.map(optionsEntity, function (option) {
                                    return $(option).attr("data-no-of-port");
                                });

                                if (portsList.includes(options[i].no_of_port.toString()) && vendorsList.includes(options[i].vendor_id.toString()))
                                    options[i].hidden = false;
                                else {
                                    options[i].hidden = true;
                                }
                            }
                       

                            //FMS check
                            if (check && radioValue == DE.FMSMap) {
                                let fmsData = $fmsEntity.data();
                                //check = false;
                                if (fmsData) {
                                    let spec = fmsData.specification;
                                    let code = fmsData.itemCode;
                                    let vendor = fmsData.vendorId;
                                    let ports = fmsData.noOfPort;
                                    if (options[i].specification == spec && options[i].code == code && options[i].no_of_port == ports && options[i].vendor_id == vendor)
                                        check = true;
                                    else {
                                        check = false;
                                        //options[i].hidden = true;
                                    }
                                } else {
                                    check = false;
                                }
                            }
                        }

                        if (searchText && check) {
                            options[i].enabled = check;
                            if (!options[i].hidden)
                                arr.push(options[i]);
                        }
                        else if (!searchText) {
                            options[i].enabled = check;
                            if (!options[i].hidden)
                                arr.push(options[i]);
                        }
                    }
                } //}
                $(DE.lblmsg).html('');
                if (arr.length == 0) {

                    if (radioValue == DE.FMSMap) {
                        $(DE.lblmsg).html("No record found for matching no. of ports and Vendor");//No record found!
                    }
                    else {
                        $(DE.lblmsg).html(MultilingualKey.SI_OSP_GBL_GBL_RPT_001);//No record found!
                    }
                  
                }
                generate.libraryList(arr);
                render.library();
            }
        },
        showSubMenuContext: function (flag) {
            if (flag)
                $(DE.subMenuContext).show();
            else
                $(DE.subMenuContext).hide();
        },
        showRoomEquipmentLib: function (flag) {
            if (flag)
                $(DE.roomEquipmentLib).show();
            else
                $(DE.roomEquipmentLib).hide();
        },
        resetTempModel: function () {
            d3.select("#" + DE.tempModel).attr("x", 0)
                                   .attr("y", 0)
                                   .attr("height", 0)
                                   .attr("width", 0)
                                    .html('');


        },
        tempModel: function (e) {
            let modelGroupFound = d3.select("#" + DE.modelGroup + e.id);
            d3.select("#" + DE.tempModel).remove();

            _$workArea.append("svg").attr('pointer-events', 'none').attr("id", DE.tempModel)
                        //.attr("x", e.parentPosition.x)
                        //.attr("y", e.parentPosition.y)
                    .append("g")
                .attr("transform", function () {
                    //if (modelGroupFound.attr("transform"))
                    //    return modelGroupFound.attr("transform");
                    //return 'translate(' + (generate.modelPositionX(e) + e.parentPosition.x) + ',' + (generate.modelPositionY(e) + e.parentPosition.y) + ') rotate(' + e.rotation_angle + "," + e.width / 2 + "," + e.height / 2 + ")";

                    let operation = generate.renderOperation(e.parent) + generate.transformText(e);
                    //console.log(operation);
                    return operation;
                })

                    .append("svg")
                    .attr("pointer-events", "none")
                        //.attr("id", DE.tempModel)
                        //.attr("x", generate.modelPositionX(e))
                        //.attr("y", generate.modelPositionY(e))
                        .attr("x", 0)
                        .attr("y", 0)
                        //.attr("height", e.height)
                        //.attr("width", e.width)
                    .style("fill-opacity", 0.3)

                        .html(e.content);
        },
        resetMultiSelection: function (e) {
            action.selectModel({});
            _selectMany = [];
            _workSpaceActions.clear();
            _workSpaceActions.clearManySelect();
        },
        showEquipmentType: function (flag) {
            if (flag)
                $(DE.equipmentType).show();
            else
                $(DE.equipmentType).hide();
        },
        showRoomViewEquipmentLibrary: function (flag) {
            flag = flag == "true";
            if (flag) {
                $(DE.rackRoom).show();
                $(DE.equipmentLib).hide();
            }
            else{
                $(DE.equipmentLib).show();
                $(DE.rackRoom).hide();
            }
        },

        showRoomViewEquipmentType: function (flag) {
            if (flag)
                $(DE.roomViewEquipmentType).show();
            else
                $(DE.roomViewEquipmentType).hide();
        },
        showFMSMap: function (flag) {
            if (flag)
                $(DE.existedFMS).show();
            else
                $(DE.existedFMS).hide();
        },
        selector: function (d) {

            let option = "";
            if (d.defaultText)
                option = "<option value disabled selected hidden>" + d.defaultText + "</option>";
            if (d && d.data) {
                let len = d.data.length;
                let keys = "";
                if (len > 0) {
                    for (let i = 0; i < len; i++) {
                        keys = "";
                        if (d.dataKey && d.dataKey.length) {
                            for (let j = 0; j < d.dataKey.length; j++) {
                                keys += " " + d.dataKey[j].name + "='" + d.data[i][d.dataKey[j].key] + "'";
                            }
                        }
                        option += "<option value='" + d.data[i][d.value] + "' " + keys + " >" + d.data[i][d.text] + "</option>";
                    }
                }
            }
            d.element.attr('disabled', d.disabled);
            d.element.html(option);
        },
        rackView: function (flag) {
            let radioValue = $(DE.libFilterOptions + ":checked").val();
            let $lnkrackView = $(DE.linkRackView);
            let $lnkroomView = $(DE.linkRoomView);
            let $rackTitle = $(DE.rackTitle);
            let $libContext = $(DE.rackLibFilterContext);
            let $roomlibContext = $(DE.roomLibFilterContext);
            render.clearEntityInfo();
            $(DE.navRoomLib).show();
            render.showConnectionCheck(flag);
            render.showConnectionMenu(flag);
            if (flag) {
                $lnkrackView.hide();
                $roomlibContext.hide()
                $lnkroomView.show();
                $("#rackViewDisplay").show();
                $rackTitle.hide();
                $libContext.show();
                render.showEquipmentType(radioValue == DE.equipmentKey);
                render.showFMSMap(radioValue == DE.FMSMap);
            }
            else {
                $lnkrackView.show();
                $lnkroomView.hide();
                let isRackLib = $(DE.roomLibFilterOptions).length > 0 ? $(DE.roomLibFilterOptions + ":checked").val():"true";
                $rackTitle.show();
                render.showRoomViewEquipmentLibrary(isRackLib);
                $roomlibContext.show()
                $libContext.hide();
                render.showEquipmentType(flag);
                render.showFMSMap(flag);
            }
        },
        workAreaSwitch: function () {
            if (validate.isRoomView()) {
                _$workArea = d3.select(DE.svgWorkArea).on("contextmenu", event.onWorkspaceContextMenu);
                $(DE.workAreaContext).show();
                $(DE.rackAreaContext).hide();
            }
            else {
                _$workArea = d3.select(DE.svgRackArea).on("contextmenu", event.onWorkspaceContextMenu);
                $(DE.workAreaContext).hide();
                $(DE.rackAreaContext).show();
            }
        },
        resetWorkArea: function () {
            let currentWorkArea = generate.currentWorkArea();
            currentWorkArea.selectAll('*').remove();
            //action.setScale();
        },
        subContextMenu: function (d, coordinates, isMultiSelect) {
            let rect = _workSpaceActions.getAbsoluteRect(d.id);
            render.showSubMenuContext(true);
            let $menuContext = $(DE.subMenuContext);
            let $grid = generate.currentWorkAreaContext();

            if (d.rack_id == 999999) {
                $menuContext.find(DE.editSubMenuContext).hide();
                //$menuContext.find(DE.savePosSubMenuContext).hide();
                $menuContext.find(DE.deleteSubMenuContext).hide();
                
            }
            else {
                $menuContext.find(DE.editSubMenuContext).show();
                //$menuContext.find(DE.savePosSubMenuContext).show();
                $menuContext.find(DE.deleteSubMenuContext).show();
            }
            //let x = coordinates[0] + 300 - $grid.scrollLeft(); //+ 325
            //let y = coordinates[1] + 110 - $grid.scrollTop();//+ 135
            //if (rect) {
            //    x = x + rect.x;
            //    y = y + rect.y;
            //}
            let x = coordinates[0] - 25;//- $grid.scrollLeft();
            let y = coordinates[1] - 25;// - $grid.scrollTop();
            $menuContext.css('left', x + 'px').css('top', y + 'px');

            //Validate action by roles
            render.saveDisabled(!action.validateEditByRights(_selectedModel.network_status));
            render.deleteDisabled(_selectedModel.db_id != 0 && !action.validateDeleteByRights(_selectedModel.network_status));
        },
        rackViewFooter: function (flag) {
            if (flag)
                $(DE.rackViewFooter).show();
            else
                $(DE.rackViewFooter).hide();
        },
        rackTabs: function () {

            let $rackTabContext = $(DE.rackTabContext);
            _workAreaData = _workSpaceActions.getWorkArea();
            let len = _workAreaData.length;
            let content = '';
            //var notInRack = 0;
            for (let i = 1; i < len; i++) {
                //if (_workAreaData[i].model_type.toUpperCase() == 'EQUIPMENT') {
                //    notInRack += 1;
                //}
                if (_workAreaData[i].db_id > 0 && _workAreaData[i].db_id != 999999 && _workAreaData[i].model_type != undefined && _workAreaData[i].model_type.toUpperCase() != 'EQUIPMENT') {
                    content += `<div class="tabview rackTab" id="rackTab${_workAreaData[i].id}" data-db-id="${_workAreaData[i].db_id}">${_workAreaData[i].name}</div>`;
                } else if (_workAreaData[i].db_id == 999999 && _workAreaData.filter(x=>x.model_type == 'Equipment').length > 0) {
                    content += `<div class="tabview rackTab" id="rackTab${_workAreaData[i].id}" data-db-id="${_workAreaData[i].db_id}">${_workAreaData[i].name}</div>`;
                }
            }
            //if (notInRack > 0) {
            //    content += `<div class="tabview equipmentTab" id="equipmentTab" data-db-id="0">Equipment Without Rack</div>`;
            //}
            $rackTabContext.html(content);
        },
        lnkRackView: function (flag) {
            if (flag)
                $(DE.linkRackView).show();
            else
                $(DE.linkRackView).hide();
        },
        lnkConnection: function (flag) {
            if (flag)
                $(DE.linkConnection).show();
            else
                $(DE.linkConnection).hide();
        },
        setRoomTitle: function (name) {
            $(DE.roomViewTitle).attr('data-original-title', name);
            $(DE.roomViewTitle).html(name);
        },
        resetInfoContext: function () {
            //let innertext = `<div class="leftPanel clsInfoTool" data-original-title="" title="" style="display: block;"><div style="width:800px;" data-original-title="" title=""><div id="dvEntityInformationList" data-original-title="" title=""><div id="divInfoNoRecordFound" class="NoRecordFound" style="color: #fff" data-original-title="" title="">No record found!</div> </div><div id="dvEntityInformationDetail" class="infodiv" data-original-title="" title=""><div class="NoRecordInfo" data-original-title="" title="">Please click on any network entity for information.</div></div></div></div>`;
            //let $infoContext = $(DE.infoContext);
            //$infoContext.html(innertext);
        },
        leftPnlToInfo: function () {
            let $leftPanel = $(DE.roomLeftPanel);
            let $info = $(DE.roomRackInfo);
            $(DE.navRoomLib).removeClass(DE.activeTabClass);
            $leftPanel.hide();
            $(DE.navRoomInfo).addClass(DE.activeTabClass);
            $info.show();
        },
        infoToLeftPnl: function () {
            let $leftPanel = $(DE.roomLeftPanel);
            let $info = $(DE.roomRackInfo);
            $(DE.navRoomLib).addClass(DE.activeTabClass);
            $leftPanel.show();
            $(DE.navRoomInfo).removeClass(DE.activeTabClass);
            $info.hide();
        },
        showEditSaveCancel: function (flag) {
            if (flag) {
                $(DE.entityCancel).parent().fadeIn();
                $(DE.entitySave).parent().fadeIn();
            }
            else {
                $(DE.entityCancel).parent().fadeOut();
                $(DE.entitySave).parent().fadeOut();
            }
        },
        roomDoor: function (param) {
            //console.log("room door rendering...");
            //console.log(param.position, param.width, param.type);
            let pos = generate.doorPostion(param);
            let content = pos.svgContent || '<path d="M50,50 v-50 a50,50 0 0,0 -50,50 z" fill="#bcbcbc" stroke="gray" stroke-width="3"/>';

            if (pos) {
                d3.select("#" + DE.roomDoor).remove();
                let doorContext = _$workArea.append("svg").attr("id", DE.roomDoor).attr("pointer-events", "visible");

                doorContext.append('title').text(param.type);
                doorContext.append("g").append("svg")
                    //.attr("preserveAspectRatio", "xMinYMin meet")
                     //.attr("preserveAspectRatio", "none")
                    .attr("id", DE.roomDoorRect)
                    .attr("viewBox", function (d) { return pos.viewBox; })
                    .attr("pointer-events", "none")
                    .attr("x", pos.x)
                    .attr("y", pos.y)
                    .attr("height", (pos.height))
                    .attr("width", (pos.width))

                    .style("fill-opacity", 1).html(content)
                ;
                //doorContext.append('rect')
                //    .attr("height", (param.width))
                //        .attr("width",(param.width))
                //    .attr('fill-color', '#0000FF')
                //.attr('stroke', '#00CCFF');

            }
        },
        rackBorder: function (e) {
            render.topBorder(e);
            render.leftBorder(e);
            render.leftBorderMark(e);
            render.rightBorder(e);
            render.bottomBorder(e);

        },
        topBorder: function (e) {
            let lWidth = e.outer_border_width;
            d3.select("#" + DE.topBorder).remove();
            let container = _$workArea.append("svg")
                    .attr("pointer-events", "none")
                    .attr("id", DE.topBorder)
                    .attr("x", generate.modelPositionX(e) - lWidth)
                    .attr("y", generate.modelPositionY(e) - 20)
                    .attr("height", 20)
                    .attr("width", e.width + (2 * lWidth));
            container.append("rect")
                .attr("pointer-events", "none")

                    .attr("x", 0)
                    .attr("y", 0)
                    .attr("height", 20)
                    .attr("width", e.width + (2 * lWidth))
                    .style('fill', function () {
                        let c = DE.outerColor;
                        if (e.border_color) c = e.border_color;
                        return c;
                    })
                    .style("fill-opacity", 1)
                    .style("stroke", "#000")
                    .style("stroke-width", "0.4");
        },
        leftBorder: function (e) {
            let lWidth = e.outer_border_width;
            d3.select("#" + DE.leftBorder).remove();
            let container = _$workArea.append("svg")
                    .attr("pointer-events", "none")
                    .attr("id", DE.leftBorder)
                    .attr("x", generate.modelPositionX(e) - lWidth)
                    .attr("y", generate.modelPositionY(e))
                    .attr("height", e.height)
                    .attr("width", lWidth);
            container.append("rect")
                .attr("pointer-events", "none")

                    .attr("x", 0)
                    .attr("y", 0)
                    .attr("height", e.height)
                    .attr("width", lWidth)
                   .style('fill', function () {
                       let c = DE.outerColor;
                       if (e.border_color) c = e.border_color;
                       return c;
                   })
                    .style("fill-opacity", 1)
                    .style("stroke", "#000")
                    .style("stroke-width", "0.4");
            //let unitData = generate.unitData(e.no_of_units, 1);
            //let lebels = container.selectAll('.l-units').data(unitData).enter();
            //lebels.append("text")
            //   .style("font-size", '14px')
            //    .style("fill", 'white')
            //   .attr("x", 15)
            //   .attr("y", function (d) { return e.height - (d * 50) + 30; })
            //.text(function (d, i) { return d + "U"; });
        },
        leftBorderMark: function (e) {
            let lWidth = 60;
            d3.select("#" + DE.leftBorderMark).remove();
            let container = _$workArea.append("svg")
                    .attr("pointer-events", "none")
                    .attr("id", DE.leftBorderMark)
                    .attr("x", 0)
                    .attr("y", generate.modelPositionY(e))
                    .attr("height", e.height)
                    .attr("width", lWidth);
            container.append("rect")
                .attr("pointer-events", "none")

                    .attr("x", 0)
                    .attr("y", 0)
                    .attr("height", e.height)
                    .attr("width", lWidth)
                   .style('fill', function () {
                       //let c = DE.outerColor;
                       //if (e.border_color) c = e.border_color;
                       //return c;
                       return 'transparent';
                   })
                    .style("fill-opacity", 1)
                    .style("stroke", "transparent")
                    .style("stroke-width", "0.4");
            let unitData = generate.unitData(e.no_of_units, 1);
            let lebels = container.selectAll('.l-units').data(unitData).enter();
            lebels.append("text")
               .style("font-size", '14px')
                .style("fill", 'black')
               .attr("x", 15)
               .attr("y", function (d) { return e.height - (d * 50) + 30; })
            .text(function (d, i) { return d + "U"; });
        },
        rightBorder: function (e) {
            d3.select("#" + DE.rightBorder).remove();
            let container = _$workArea.append("svg")
                    .attr("pointer-events", "none")
                    .attr("id", DE.rightBorder)
                    .attr("x", generate.modelPositionX(e) + e.width)
                    .attr("y", generate.modelPositionY(e))
                    .attr("height", e.height)
                    .attr("width", e.outer_border_width);
            container.append("rect")
                .attr("pointer-events", "none")

                    .attr("x", 0)
                    .attr("y", 0)
                    .attr("height", e.height)
                    .attr("width", e.outer_border_width)
                    .style('fill', function () {
                        let c = DE.outerColor;
                        if (e.border_color) c = e.border_color;
                        return c;
                    })
                    .style("fill-opacity", 1)
                    .style("stroke", "#000")
                    .style("stroke-width", "0.4");
        },
        bottomBorder: function (e) {
            let lWidth = e.outer_border_width;
            d3.select("#" + DE.bottomBorder).remove();
            let container = _$workArea.append("svg")
                    .attr("pointer-events", "none")
                    .attr("id", DE.bottomBorder)
                    .attr("x", generate.modelPositionX(e) - lWidth)
                    .attr("y", generate.modelPositionY(e) + e.height)
                    .attr("height", 60)
                    .attr("width", e.width + (2 * lWidth));
            container.append("rect")
                .attr("pointer-events", "none")

                    .attr("x", 0)
                    .attr("y", 0)
                    .attr("height", 20)
                    .attr("width", e.width + (2 * lWidth))
                    .style('fill', function () {
                        let c = DE.outerColor;
                        if (e.border_color) c = e.border_color;
                        return c;
                    })
                    .style("fill-opacity", 1)
                    .style("stroke", "#000")
                    .style("stroke-width", "0.4");
            container.append("rect")
               .attr("pointer-events", "none")

                   .attr("x", 0)
                   .attr("y", 20)
                   .attr("height", 40)
                   .attr("width", 40)
                   .style('fill', function () {
                       let c = DE.outerColor;
                       if (e.border_color) c = e.border_color;
                       return c;
                   })
                   .style("fill-opacity", 1)
                   .style("stroke", "#000")
                   .style("stroke-width", "0.4");
            container.append("rect")
               .attr("pointer-events", "none")

                   .attr("x", e.width - 40 + (2 * lWidth))
                   .attr("y", 20)
                   .attr("height", 40)
                   .attr("width", 40)
                  .style('fill', function () {
                      let c = DE.outerColor;
                      if (e.border_color) c = e.border_color;
                      return c;
                  })
                   .style("fill-opacity", 1)
                   .style("stroke", "#000")
                   .style("stroke-width", "0.4");
        },
        libChildren: function (res) {

            if (res.length > 1) {
                _workAreaData = _workSpaceActions.getWorkArea();
                res.splice(0, 1);
                let selected = _workSpaceActions.getCurrent();
                //Remove Cyclic reference 
                let maxId = _workSpaceActions.getMaxId();

                res.map(function (obj) {
                    if (obj.id > maxId) maxId = obj.id;
                });
                maxId++;
                for (let i = 0; i < res.length; i++) {
                    let children = res.filter(x=>x.parent == res[i].id);
                    for (let j = 0; j < children.length; j++) {
                        children[j].parent = maxId;
                    }
                    res[i].id = maxId;
                    maxId++;
                }

                for (let i = 0; i < res.length; i++) {
                    if (0 == res[i].parent) {
                        res[i].parent = selected.id;
                        res[i].is_static = true;
                    }
                    _workAreaData.push(res[i]);
                }

            } else {

            }
            render.allModels();

        },
        scaleText: function (val) {
            $(DE.scaleText).text(val);
        },
        activeRackTab: function ($selected) {
            $(DE.rackTab).removeClass(DE.rackActive);
            $selected.addClass(DE.rackActive);
        },
        infoProgress: function (divId) {
            var infoProgress = '<div id="dvInfoProgress" style="display: block;"><div id="blur" class="infoProgresblur">&nbsp;</div><div id="infoProgressbar" style="width: 247px; height: 152px; top: 40%; left: 5%;">';
            infoProgress += '<img alt="Loading..." src="../content/images/loading_new.gif" /><br /></div></div>';
            $(divId).html(infoProgress);
        },
        setInView: function (model, parent) {
            let rect = parent || _$workArea.node().getBoundingClientRect()
            let $grid = generate.currentWorkAreaContext();

            let pos_x = ($grid.width() - model.width) / 2 + $grid.scrollLeft(), pos_y = ($grid.height() - model.height) / 2 + $grid.scrollTop();;
            if (rect.width < $grid.width()) {
                pos_x = rect.width / 2 - model.width / 2;
            }
            if (rect.height < $grid.height()) {
                pos_y = rect.height / 2 - model.height / 2;
            }
            model.position.x = (pos_x < 0 ? 0 : pos_x) + (model.width / 2);
            model.position.y = (pos_y < 0 ? 0 : pos_y) + (model.height / 2);
        },
        roomBorder: function (e) {
            d3.select("#" + DE.roomBorder).remove();
            let container = _$workArea.append("svg");
            container.append('rect')
                    .attr("pointer-events", "none")
                    .attr("x", generate.modelPositionX(e) - 5)
                    .attr("y", generate.modelPositionY(e) - 5)
                    .attr("height", (e.height + 10))
                    .attr("width", (e.width + 10))
                    .style("fill", "transparent")
                    .style("stroke", "#000")
                    .style("stroke-width", 10).style("fill-opacity", 1);
        },
        showSavePos: function (flag) {
            if (flag)
                $(DE.savePosSubMenuContext).show();
            else
                $(DE.savePosSubMenuContext).hide();
        },
        equipmentStatus: function (e) {
            d3.selectAll("." + DE.eqStatusBorder + e.id).remove();
            let container = _$workArea;
            let h = 16, w = 16;
            let rect = _workSpaceActions.getAbsoluteRect(e.id);
            //top left
            container.append("svg").attr("pointer-events", "none")
                        .attr("x", rect.x)
                        .attr("y", rect.y)
                        .attr("height", (h))
                        .attr("width", (w))
                         .attr('class', DE.eqStatusBorder + e.id)
                        .html(equipmentStatsSvg.topLeft.replaceAll('[COLOR]', networkEqColor[e.network_status]).replaceAll('[BCOLOR]', networkRackColor[e.network_status]));
            //top right
            container.append("svg").attr("pointer-events", "none")
                        .attr("x", rect.x + rect.width - w)
                        .attr("y", rect.y)
                        .attr("height", (h))
                        .attr("width", (w))
                        .attr('class', DE.eqStatusBorder + e.id)
                        .html(equipmentStatsSvg.topRight.replaceAll('[COLOR]', networkEqColor[e.network_status]).replaceAll('[BCOLOR]', networkRackColor[e.network_status]));
            //bottom left
            container.append("svg").attr("pointer-events", "none")
                        .attr("x", rect.x)
                        .attr("y", rect.y + rect.height - h)
                        .attr("height", (h))
                        .attr("width", (w))
                  .attr('class', DE.eqStatusBorder + e.id)
                        .html(equipmentStatsSvg.bottomLeft.replaceAll('[COLOR]', networkEqColor[e.network_status]).replaceAll('[BCOLOR]', networkRackColor[e.network_status]));
            //bottom right
            container.append("svg").attr("pointer-events", "none")
                   .attr("x", rect.x + rect.width - w)
                        .attr("y", rect.y + rect.height - h)
                        .attr("height", (h))
                        .attr("width", (w))
                  .attr('class', DE.eqStatusBorder + e.id)
                        .html(equipmentStatsSvg.bottomRight.replaceAll('[COLOR]', networkEqColor[e.network_status]).replaceAll('[BCOLOR]', networkRackColor[e.network_status]));
        },
        allEquipmentStatus: function () {
            let allEq = _workAreaData.filter(x=> x.parent == 1);
            let len = allEq.length;
            for (let i = 0; i < len; ++i) {
                render.equipmentStatus(allEq[i]);
            }
        },
        showISPView: function (flag) {
            if (flag) {
                $(DE.tabIspView).show();
                $(DE.ispPanel).show();
            }
            else {
                $(DE.tabIspView).hide();
                $(DE.ispPanel).hide();
            }
        },
        showRoomViewClose: function (flag) {
            if (flag) {
                $(DE.closeRoomView).show();

            }
            else {
                $(DE.closeRoomView).hide();

            }
        },
        modelSaveText: function (flag) {
            let $save = $(DE.rackSave);
            if (flag) {

                $save.html(`<i class="fa fa-save"></i>` + MultilingualKey.GBL_GBL_GBL_GBL_GBL_004);

            }
            else {

                $save.html(`<i class="fa fa-edit"></i> ` + MultilingualKey.SI_GBL_GBL_JQ_FRM_016);
            }

        },
        editPosition: function (flag) {
            let $editPos = $(DE.editSubMenuContext);
            let $savePos = $(DE.savePosSubMenuContext);
            let $revertPos = $(DE.revertPosSubMenuContext);
            if (flag) {
                $editPos.hide();
                $savePos.show();
                $revertPos.show();
            }
            else {
                $editPos.show();
                $savePos.hide();
                $revertPos.hide();
            }
        },
        saveDisabled: function (flag) {
            let $rackSave = $(DE.rackSave);
            let $editPos = $(DE.editSubMenuContext);
            let $savePos = $(DE.savePosSubMenuContext);
            $rackSave.prop('disabled', flag);
            $editPos.prop('disabled', flag);
            if (flag) {
                $rackSave.addClass(DE.roleDisabledClass);
                $editPos.addClass(DE.roleDisabledClass);
                $savePos.addClass(DE.roleDisabledClass);
            }
            else {
                $rackSave.removeClass(DE.roleDisabledClass);
                $editPos.removeClass(DE.roleDisabledClass);
                $savePos.removeClass(DE.roleDisabledClass);
            }
        },
        deleteDisabled: function (flag) {
            let $del = $(DE.deleteSubMenuContext);
            $del.prop('disabled', flag);
            if (flag) {
                $del.addClass(DE.roleDisabledClass);
            }
            else { $del.removeClass(DE.roleDisabledClass); }
        },
        showConnectionCheck: function (flag) {
            if (flag) {
                $(DE.connectionContext).show();

            }
            else {
                $(DE.connectionContext).hide();

            }
        },

        showConnectionMenu: function (flag) {
            if (flag) {
                $(DE.viewConSubMenuContext).show();

            }
            else {
                $(DE.viewConSubMenuContext).hide();

            }
        },
        setPopDetail: function (detail) {
            $(DE.ispPOPId).val(detail.systemId);
            $(DE.ispPOPType).val(detail.entityType);

        },
        showConnectedWire: function (_target, _portType) {
            let _workAreaData = _workSpaceActions.getWorkArea();
            let _rackData = _workAreaData[0];
            let _wireColor = 'green';
            let _animatedClass = 'portAnimationGreen';
            if (_portType == 'I') { _wireColor = '#800'; _animatedClass = 'portAnimationRed'; }
            let targetRect = _workSpaceActions.getAbsoluteRect(_target.id);
            let container = d3.select('#' + DE.ElementLine);
            container = container.append("g");
            container.append('path')
                .attr('d', function () { return generate.linePath(_selectedModel, _target) })
                .attr('stroke', '#ccc')
                .attr('stroke-width', 2)
                .attr('fill', 'none');

            container.append('path')
                .attr('d', function () { return generate.linePath(_selectedModel, _target) })
                .attr('stroke', _wireColor)
                .attr('stroke-width', 2)
                .attr('fill', 'none')
                .attr('class', _animatedClass);
            //.on('click', event.onWireMouseOver);
            //.on('click', event.onWireMouseOut);

            let targetCircle = container.append("g");
            targetCircle.append('circle')
              .attr('fill', 'green')
              .attr('cx', function () { return ((targetRect.x + targetRect.width / 2) - generate.modelPositionX(_rackData)); })
              .attr('cy', function () { return ((targetRect.y + targetRect.height / 2) - generate.modelPositionY(_rackData)); })
              //.attr('cx', function () { return (targetRect.x - (_target.width / 2 + _rackData.outer_border_width)); })
              //.attr('cy', function () { return (targetRect.y - _target.height / 2); })
              .attr('r', 4);
        },
        showOutConnection: function () {
            let _workAreaData = _workSpaceActions.getWorkArea();
            let _rackData = _workAreaData[0];
            let _wireColor = 'green';
            let _animatedClass = 'portAnimationGreen';
            let container = d3.select('#rackViewContext');
            container = container.append("g").attr("id", DE.OuterElementLine).attr("transform", function (d) {
                return 'translate(' + generate.modelPositionX(_rackData) + ',' + generate.modelPositionY(_rackData) + ") rotate(0," + _rackData.width / 2 + "," + _rackData.height / 2 + ")";
            });
            container.append('path')
                .attr('d', function () { return generate.outLinePath(); })
                .attr('stroke', '#ccc')
                .attr('stroke-width', 2)
                .attr('fill', 'none');

            container.append('path')
                .attr('d', function () { return generate.outLinePath(); })
                .attr('stroke', _wireColor)
                .attr('stroke-width', 2)
                .attr('fill', 'none').attr('class', _animatedClass);

            let position = generate.outConnectionTarget();
            let targetCircle = container.append("g");
            targetCircle.append('circle')
              .attr('fill', 'green')
              .attr('cursor', 'pointer')
              .attr('cx', function () { return position.x; })
              .attr('cy', function () { return position.y; })
              .attr('r', 4).on('click', event.onOutConnectionPointClick);

            let targetOuterrrRing = container.append("g");
            targetOuterrrRing.append('circle')
              .attr('fill', 'none')
                .attr('stroke-width', 2)
                .attr('stroke', '#ccc')
              .attr('cx', function () { return position.x; })
              .attr('cy', function () { return position.y; })
              .attr('r', 8);
        },
        showConnectionView: function (flag) {
            if (!flag) {
                d3.select("#" + DE.ElementLine).remove();
                d3.select("#" + DE.OuterElementLine).remove();
                _connectionView = false;
            }
        },
        showEditorSubMenuContext: function (flag) {
            if (flag) { $(DE.EquipmentViewSubMenuContext).show(); } else { $(DE.EquipmentViewSubMenuContext).hide();}
        }
    };

    var validate = {
        modelDrop: function (param) {
            let count = _hierarchyRules.filter(x=> x.parent_model_id == param.parent_model_id
                                    && x.parent_model_type_id == param.parent_model_type_id
                                    && x.child_model_id == param.child_model_id
                                    && x.child_model_type_id == param.child_model_type_id);
            //return count.length;
            return true;
        },
        inBoundary: function (id, point) {
            let flag = false;
            //let rect = _workSpaceActions.getAbsoluteRect(id);

            // _workAreaData = _workSpaceActions.getWorkArea();
            if (_workSpaceActions.contains(id, point)) {
                flag = true;
            }
            return flag;
        },
        isRoomView: function () {

            return _isRoomView;
        },
        collision: function (id, param) {
            _workAreaData = _workSpaceActions.getWorkArea();
            let current = _workSpaceActions.select(id);
            //console.log(current);
            let rect = _workSpaceActions.getAbsoluteRect(current.parent);

            let tempRect = { x: rect.x + param.x - current.width / 2, y: rect.y + param.y - current.height / 2, height: current.height, width: current.width };
            switch (current.rotation_angle) {
                case 90:
                case 270:
                    tempRect = {
                        x: rect.x + param.x - current.height / 2,
                        y: rect.y + param.y - current.width / 2,
                        height: current.width,
                        width: current.height
                    };
                    break;
            }
            let sibling = _workAreaData.filter(x=> x.id != id && x.id > 1 && x.parent == 1);
            let len = sibling.length;
            //console.log(p1, p2, p3, p4);
            for (let i = 0 ; i < len; i++) {
                let sRect = _workSpaceActions.getAbsoluteRect(sibling[i].id, true);
                if (_workSpaceActions.isRectOverlapped(tempRect, sRect)) {
                    return true;
                }
            }
            //check for door
            let $doorRect = $('#' + DE.roomDoorRect);
            if ($doorRect.length > 0) {
                let sRect = { x: parseFloat($doorRect.attr('x')), y: parseFloat($doorRect.attr('y')), width: parseFloat($doorRect.attr('width')), height: parseFloat($doorRect.attr('height')) }
                if (_workSpaceActions.isRectOverlapped(tempRect, sRect)) {
                    return true;
                }
            }
            return false;
        },
        isNavRoomLib: function () {
            let $roomLib = $(DE.navRoomLib);
            return $roomLib.hasClass(DE.activeTabClass);
        },
        isStucture: function () {
            let $structure = $(DE.StructureId);
            if ($structure.length) {
                let structureId = $structure.val();
                if (structureId && structureId > 0)
                    return true;
            }
            return false;
        },
        isRoomInfo: function () {
            return $(DE.roomRackInfo).is(":visible");
        },
        isNoNewEquipment: function () {
            return _workSpaceActions.newModelCount() == 0;
        },
        isValidPortCount: function (count, parent) {
            parent = parent == undefined ? _selectedModel.id : parent;
            return count == generate.noOfPorts(parent);
        },
        isFMSMapping: function () {
            let isMapping = false;
            let radioValue = $(DE.libFilterOptions + ":checked").val();
            let $fmsEntity = $(DE.selectExistedFms + " option:selected");
            if (radioValue == DE.FMSMap) {
                if ($fmsEntity.length && $fmsEntity.val())
                    isMapping = true;
            }
            return isMapping;
        },
        addRights: function (entityType, networkStatus) {
            let rights = (entityType.toLowerCase() == 'rack') ? DE.roleAccess.rack : DE.roleAccess.equipment;
            if (networkStatus == 'P') {
                return rights.planned_add.toLowerCase() == 'true';
            }
            if (networkStatus == 'A') {
                return rights.asbuild_add.toLowerCase() == 'true';
            }
            if (networkStatus == 'D') {
                return rights.dormant_add.toLowerCase() == 'true';
            }
        },
        editRights: function (entityType, networkStatus) {
            let rights = (entityType.toLowerCase() == 'rack') ? DE.roleAccess.rack : DE.roleAccess.equipment;
            if (networkStatus == 'P') {
                return rights.planned_edit.toLowerCase() == 'true';
            }
            if (networkStatus == 'A') {
                return rights.asbuild_edit.toLowerCase() == 'true';
            }
            if (networkStatus == 'D') {
                return rights.dormant_edit.toLowerCase() == 'true';
            }
        },
        deleteRights: function (entityType, networkStatus) {
            let rights = (entityType.toLowerCase() == 'rack') ? DE.roleAccess.rack : DE.roleAccess.equipment;
            if (networkStatus == 'P') {
                return rights.planned_delete.toLowerCase() == 'true';
            }
            if (networkStatus == 'A') {
                return rights.asbuild_delete.toLowerCase() == 'true';
            }
            if (networkStatus == 'D') {
                return rights.dormant_delete.toLowerCase() == 'true';
            }
        },
        viewRights: function (entityType, networkStatus) {
            let rights = (entityType.toLowerCase() == 'rack') ? DE.roleAccess.rack : DE.roleAccess.equipment;
            if (networkStatus == 'P') {
                return rights.planned_view.toLowerCase() == 'true';
            }
            if (networkStatus == 'A') {
                return rights.asbuild_view.toLowerCase() == 'true';
            }
            if (networkStatus == 'D') {
                return rights.dormant_view.toLowerCase() == 'true';
            }
        },
        isOutConnectionFound: function (ports) {
            let _isOutConnected = false;
            _workAreaData = _workSpaceActions.getWorkArea();
            for (var i = 0; i < ports.length; i++) {
                let _tagertDetails = _workAreaData.filter(x=>x.db_id == ports[i].destination_system_id && x.model_id == ports[i].model_id);
                _isOutConnected = _tagertDetails.length == 0;
            }
            return _isOutConnected;
        }
    };

    var bind = {
        tabPanel: function () {
            let $tabIspView = $(DE.tabIspView);
            $tabIspView.on('click', event.onIspTabClick);

            let $tabRoomView = $(DE.tabRoomView);
            $tabRoomView.on('click', event.onIspTabClick);

            let $infoContext = $(DE.infoContext);
            // let $entityRoomView = $(DE.entityRoomView);
            $infoContext.on('click', DE.entityRoomView, event.onRoomViewClick);

            let $floorRoom = $(DE.InfoRoomViewFloor);
            // let $entityRoomView = $(DE.entityRoomView);
            $(document).on('click', DE.entityRoomViewFloor, event.onRoomViewClick);

            let $closeRoomView = $(DE.closeRoomView);
            $closeRoomView.on('click', event.onRoomViewCloseClick);

            let $libFilterContext = $(DE.libFilterContext);
            $libFilterContext.on('change', DE.libFilterOptions, event.onLibFilterOptionClick);

            $libFilterContext.on('change', DE.roomLibFilterOptions, event.onRoomLibFilterOptionClick);

            let $selectLibTypes = $(DE.selectLibTypes);
            $selectLibTypes.on('change', event.onChangeSelectLibTypes);

            let selectRoomViewLibTypes = $(DE.selectRoomViewLibTypes);
            selectRoomViewLibTypes.on('change', event.onChangeSelectRoomViewLibTypes);

            
            let $txtSearchLibraryData = $(DE.txtSearchLibraryData);
            $txtSearchLibraryData.on('keyup', event.onSearchLibraryData);

            let $txtSearchRoomViewLibraryData = $(DE.txtSearchRoomViewLibraryData);
            $txtSearchRoomViewLibraryData.on('keyup', event.onSearchRoomViewLibraryData);

            let $linkRackView = $(DE.linkRackView);
            $linkRackView.on('click', event.onlnkRackView);

            let $linkRoomView = $(DE.linkRoomView);
            $linkRoomView.on('click', event.onlnkRoomView);

            let $navRoomLib = $(DE.navRoomLib);
            $navRoomLib.on('click', event.onNavRoomLib);

            let $navRoomInfo = $(DE.navRoomInfo);
            $navRoomInfo.on('click', event.onNavRoomInfo);

            //let $selectModelType = $(DE.selectModelType);
            $(document).on('change', DE.selectModelType, event.onModelTypeChange);

            let $selectExistedFms = $(DE.selectExistedFms);
            $selectExistedFms.on('click', event.onSelectUnmappedFMS);
        },
        rackOperation: function () {
            let $rackCancel = $(DE.rackCancel);
            $rackCancel.on('click', event.onCancelRackSubMenu);

            let $rackSave = $(DE.rackSave);
            $rackSave.on('click', event.onSaveRackSubMenu);

            let $deleteSubMenuContext = $(DE.deleteSubMenuContext);
            $deleteSubMenuContext.on('click', event.onDeleteSubMenuContext);

            let $editSubMenuContext = $(DE.editSubMenuContext);
            $editSubMenuContext.on('click', event.onEditSubMenu);

            let $savePosSubMenuContext = $(DE.savePosSubMenuContext);
            $savePosSubMenuContext.on('click', event.onSavePosition);

            let $revertPosSubMenuContext = $(DE.revertPosSubMenuContext);
            $revertPosSubMenuContext.on('click', event.onRevertPosSubMenuContext);

            let $viewConSubMenuContext = $(DE.viewConSubMenuContext);
            $viewConSubMenuContext.on('click', event.onConnectionViewContextMenu);

            let $infoContext = $(DE.roomInfoContext);
            //$infoContext.on('click', DE.deleteEntity, event.onDeleteSubMenuContext);
            //$infoContext.on('click', DE.entityLocationEdit, event.onEditRack);
            //$infoContext.on('click', DE.entitySave, event.onSaveRackSubMenu);
            //$infoContext.on('click', DE.entityCancel, event.onCancelEditRack);
            $infoContext.on('click', DE.entityRackView, event.onInfoRackView);

            let $chkShowConnections = $(DE.chkShowConnections);
            $chkShowConnections.on('change', event.onConnectionCheck);
          
  //          $(document).on("click", "#frmConnectionFilter :submit", function () {

  //              setTimeout(
  //function () {
  //     
  //    var a = $(".splicingLegendSection").closest("#dvModalBody");
  //    var b = a.closest(".modal-content");
  //    b.find("#closeModalPopup").on("click", function () {
  //        event.onConnectionCheck
  //    });
            //}, 5000);
          //  });

            let $closeModalPopup = $(DE.closeModalPopup);
            $(document).on('click', DE.closeModalPopup, event.onConnectionCheck);



             

            


        

            let $EquipmentViewSubMenuContext = $(DE.EquipmentViewSubMenuContext);
            $EquipmentViewSubMenuContext.on('click', event.onEquipmentViewSubMenu);
        },
        rackEvent: function () {
            let $rackTabContext = $(DE.rackTabContext);
            $rackTabContext.on('click', DE.rackTab, event.onClickRackTab);
            $rackTabContext.on('click', DE.equipmentTab, event.onClickRackTab);
        },
        imagePanel: function () {
            let $context = $(DE.roomRackInfo);
            $context.on('click', DE.liImage, event.onGetElementImages);
            $context.on('change', DE.uploadImage, event.onUploadImageFile);
            $context.on('click', DE.deleteImages, event.onDeleteImages);
            $context.on('click', DE.downloadImages, event.onDownloadImages);

        },
        documentPanel: function () {
            let $context = $(DE.roomRackInfo);
            $context.on('click', DE.liDocument, event.onGetAttachmentFiles);
            $context.on('change', DE.uploadDocument, event.onUploadDocument);
            $context.on('click', DE.deleteDocuments, event.onDeleteDocument);
            $context.on('click', DE.downloadDocuments, event.onDownloadDocument);
        },
        layerActions: function () {
            let $doc = $(DE.roomInfoContext);
            $doc.on('click', DE.EntityExport, layerActions.exportData.download);
            //$doc.on('click', DE.EntityExport, function () { $('#SourceType').val('Eqp'); action.ExportRoomViewDetail('false') });
            $doc.on('click', DE.EntityExportWithoutConnection, function () { $('#SourceType').val('Equipment'); ExportRoomViewDetail('false'); $('#SourceType').val('Rack'); });
            $doc.on('click', DE.EntityExportWithConnection, function () { $('#SourceType').val('Equipment'); ExportRoomViewDetail('true'); $('#SourceType').val('Rack'); });


            $doc.on('click', DE.EntityHistory, layerActions.history.get);

            $doc.on('click', DE.EntityDetail, layerActions.detail.get);
            $doc.on('click', DE.EntityCustomerDetails, layerActions.ConnectedCustomerDetails.get);

        },
        documentEvent: function () {
            event.onDocumentKeyPress();

        },
        refLinkPanel: function () {
             
            let $context = $(DE.roomRackInfo);
            $context.on('click', DE.liRefLink, event.ongetRefLinksFiles);
            $context.on('click', DE.uploadRefLink, event.onuploadRefLink);
            $context.on('click', DE.uploadSaveRefLink, event.onuploadSaveRefLink);
            //$context.on('click', DE.deleteDocuments, event.onDeleteDocument);
            //$context.on('click', DE.downloadDocuments, event.onDownloadDocument);
        },
    };

    var load = {
        workArea: function () {
            _$workArea = d3.select(DE.svgWorkArea).on("contextmenu", event.onWorkspaceContextMenu);
            _$libArea = d3.select(DE.svgLibArea).on("contextmenu", event.onWorkspaceContextMenu);
            _$libRoomViewArea = d3.select(DE.svgRoomViewLibArea).on("contextmenu", event.onWorkspaceContextMenu);
            _workSpaceActions.reset();
            _workAreaData = _workSpaceActions.getWorkArea();
            if (current_parent.systemId && current_parent.systemId != '' && current_parent.systemId != '0') {
                API.call(urls.getRoomSpaceData, { parentId: current_parent.systemId, parent_type: current_parent.entityType}, function (res) {
                    render.lnkRackView(res.length);
                    render.lnkConnection(res.length);
                    _workAreaData = _workAreaData.concat(res);
                     
                    _workSpaceActions.setWorkArea(_workAreaData);

                    render.allModels();
                });
            }
        },
        libArea: function () {
            API.call(urls.getRacksData, {}, function (res) {
                _libraryAllData = res;
                event.onSearchLibraryData();
                event.onSearchRoomViewLibraryData();
                //generate.libraryList(res);
                //render.library();
            });
            API.call(urls.getRackLibrary, {}, function (res) {
                _rackLibraryData = res;
                event.onSearchLibraryData();
                event.onLibFilterOption();
                //generate.libraryList(res);
                //render.library();
            });
            API.call(urls.getModelType, { modelId: DE.equipmentKey }, function (res) {
                _equipmentTypeData = res;
                _equipmentTypeData.unshift({ id: 'all', value: 'All', key: 'all' });
                //generate.libraryList(res);
                //render.library();
            });
        },
        scale: function () {
            action.setScale();
        },
        roomView: function () {

            if (!validate.isStucture()) {
                render.showISPView(false);
                render.showRoomViewClose(false);
                event.onRoomViewClick();
            }
        }
    };

    var layerActions = {

        networkStatus: {

            convertStatus: function (_systemId, _entityType, _networkStatus, _oldStatus) {
                 
                var _old = layerActions.networkStatus.getNetStatus(_oldStatus);
                var _new = layerActions.networkStatus.getNetStatus(_networkStatus);
                var layerTitle = getLayerTltle(_entityType);
                //Are you sure you want convert
                 
                confirm(getMultilingualStringValue($.validator.format(MultilingualKey.SI_ISP_GBL_JQ_GBL_020, layerTitle, _old, _new)), function () {
                    ajaxReq('Main/NetworkStage', { systemid: _systemId, entity_type: _entityType, curr_status: _networkStatus, old_status: _oldStatus }, true, function (resp) {
                        if (resp.status == 'OK') {
                            alert($.validator.format(MultilingualKey.SI_ISP_GBL_JQ_GBL_021, layerTitle, _old, _new), 'success', 'success');
                            _selectedModel.network_status = _networkStatus;
                            action.entityInformation(_selectedModel);
                            render.allModels();
                        }
                        else {
                            alert(resp.results.message, 'warning', 'Information');
                        }

                    }, false, true, false);
                });
            },
            convertToPlanned: function (_systemId, _entityType, _oldStatus) {
                layerActions.networkStatus.convertStatus(_systemId, _entityType, 'P', _oldStatus);
            },
            convertToAsBuilt: function (_systemId, _entityType, _oldStatus) {
                layerActions.networkStatus.convertStatus(_systemId, _entityType, 'A', _oldStatus);
            },
            convertToDormant: function (_systemId, _entityType, _oldStatus) {
                layerActions.networkStatus.convertStatus(_systemId, _entityType, 'D', _oldStatus);
            },
            getNetStatus: function (Status) {
                var network = '';
                if (Status == 'P')
                    network = 'Planned';
                else if (Status == 'A')
                    network = 'As-Built';
                else
                    network = 'Dormant';
                return network;
            },
            bind: function (systemID, eName, nStatus) {
                let $doc = $(DE.roomInfoContext);
                $doc.off('click', DE.EntityConvertToPlanned);
                $doc.off('click', DE.EntityConvertToAsBuilt);
                $doc.off('click', DE.EntityConvertToDormant);
                if (nStatus == 'P') {
                    $doc.find(DE.EntityConvertToPlanned).parent().hide();
                }
                else {
                    $doc.on('click', DE.EntityConvertToPlanned, function (e) {
                        if ($(e.target).hasClass("roledisabled")) {
                            return false;
                        }
                        layerActions.networkStatus.convertToPlanned(systemID, eName, nStatus);
                    });
                }
                if (nStatus == 'A' || nStatus == 'D') {
                    $doc.find(DE.EntityConvertToAsBuilt).parent().hide();
                }
                else {
                    $doc.on('click', DE.EntityConvertToAsBuilt, function (e) {
                        if ($(e.target).hasClass("roledisabled")) {
                            return false;
                        }
                        layerActions.networkStatus.convertToAsBuilt(systemID, eName, nStatus);
                    });
                }
                if (nStatus == 'D') {
                    $doc.find(DE.EntityConvertToDormant).parent().hide();
                }
                else {
                    $doc.on('click', DE.EntityConvertToDormant, function (e) {
                        if ($(e.target).hasClass("roledisabled")) {
                            return false;
                        }
                        layerActions.networkStatus.convertToDormant(systemID, eName, nStatus);
                    });
                }
            }
        },
        history: {
            get: function (e) {
                var attr = $(e.currentTarget).data();
                layerActions.history.render(attr.systemId, attr.entityType);
            },
            render: function (systemId, entityType) {
                var formURL = "Audit/GetHistory";
                var layerTitle = getLayerTltle(entityType);
                var titleText = layerTitle.toUpperCase() + " History";
                popup.LoadModalDialog('PARENT', formURL, { systemId: systemId, eType: entityType }, titleText, 'modal-lg');
            }
        },
        exportData: {
            download: function (e) {
                var attr = $(e.currentTarget).data();
                layerActions.exportData.render(attr.systemId, attr.entityType, attr.geomType);
            },

            render: function (systemID, entityType, geomType) {
                var _networkstage = '';
                ajaxReq('Main/CheckEntityData', { systemId: systemID, entityType: entityType, networkStage: '' }, false, function (status) {
                    if (status != null && status != undefined) {
                        if (status)  // check if true
                        {
                            window.location = appRoot + 'Main/ExportInfoEntity?systemId=' + systemID + '&entityType=' + entityType + '&networkStage=' + _networkstage + '';
                        }
                        else {
                            alert(MultilingualKey.SI_OSP_GBL_JQ_RPT_014, "warning");
                        }
                    }
                }, true, true);

            }
        },
        detail: {
            get: function (e) {
                var attr = $(e.currentTarget).data();
                var NetworkStatus = '';
                layerActions.detail.render({ systemId: attr.systemId, entityType: attr.entityType, geomType: attr.geomType, networkStatus: NetworkStatus });
            },
            render: function (_data) {
                var modelClass = getPopUpModelClass(_data.entityType);
                var lyrDetail = getLayerDetail(_data.entityType);
                var formURL = lyrDetail['layer_form_url'];
                var titleText = lyrDetail['layer_title'];
                var strucIdVal = $(DE.StructureId).val();
                var _model = { ModelInfo: { structureid: strucIdVal } }
                var _data = $.extend(_data, _model);
                popup.LoadModalDialog('PARENT', formURL, _data, titleText, modelClass);
            }
        },
        ConnectedCustomerDetails: {
            get: function (e) {
                 
                var attr = $(e.currentTarget).data();
                layerActions.ConnectedCustomerDetails.render({ equipment_id: attr.network_id, objFilterAttributes: { entity_type: attr.entityType, entityid: attr.systemId }, isControllEnable: false });
            },
            render: function (_data) {
                 
                // var _data = $.extend(_data);
                var formURL = 'splicing/GetConnectedCustomerDetailsInInfo';
                var titleText = 'Connected Customer Details';
                popup.LoadModalDialog('PARENT', formURL, _data, titleText, 'modal-lg');
            }
        },

        entity: {
            remove: function (e) {

            },
            deleteEntity: function (systmId, eType, eTitle, gType) {

            },
            edit: function (e) {

            },
            save: function (e) {


            },
            cancel: function (e) {

            },
            select: function (element) {
                let $res = null;
                element.type = element.type.toUpperCase();
                switch (element.type) {
                    case 'CABLE':
                        $res = $('#ispCable_' + element.systemId);
                        break;
                    default:
                        $res = $('#div_' + element.type.toUpperCase() + '_' + element.systemId);
                        break;

                }
                return $res;
            },
            setNetworkStatus: function ($e, element) {
                element.type = element.type.toUpperCase();
                switch (element.type) {
                    case 'CABLE':
                        let tempClass = $e.attr("class");
                        tempClass = tempClass.replace(element.oldStatus, '').trim();
                        $e.attr("class", tempClass + " " + element.newStatus);
                        //$e.removeClass(element.oldStatus);
                        //$e.addClass(element.newStatus);
                        break;
                    default:
                        $e.removeClass('network-status-' + element.oldStatus);
                        $e.addClass('network-status-' + element.newStatus);
                        break;

                }
            },
            focus: function ($e) {
                layerActions.entity.resetFocus($('.entityInfo'));
                $e.attr("class", $e.attr("class").trim() + " " + DE.EntitySelected);
            },
            resetFocus: function ($e) {
                $e.each(function (i, e) {
                    $(e).attr("class", $(e).attr("class").replace(DE.EntitySelected, ''));
                    $(e).attr("class", $(e).attr("class").trim().replace(DE.PathSelected, ''));
                });
            },
            roomView: function (e) {
                //open room view
            }
        }
    };
    var setup = function () {

        //Load Data
        for (let e in load) {
            if (typeof load[e] == 'function')
                load[e]();
        }
        //Bind events
        for (let e in bind) {
            if (typeof bind[e] == 'function')
                bind[e]();
        }
    };
    var init = function (configs) {
        $.extend(true, DE, configs);
        setup();

    };
    return {
        init: init,
        setNewId: action.setNewId,
        showOnRack: action.showOnRack,
        workSpaceActions: _equipmentTypeData,
        uploadRefLinkRoom: action.uploadRefLinkRoom,
        uploadReferenceLink: action.uploadReferenceLink,
        getRefLinksFiles: action.getRefLinksFiles,
        getAttachmentFiles: action.getAttachmentFiles,
        getElementImages: action.getElementImages
    };




})();

function ExportRoomViewDetail(IsConnectionFilter, exportType) {
     
    $("canvas").remove();
    $('.FinalResult').empty();
    var SourceId = "";
    var targetElem = "";
    var is_source_connected = IsConnectionFilter;
    var SourceType = $('#SourceType').val();
    var parentType = $('#ParentType').val();
    var parentId = $('#ParentID').val();
    showProgress();
    var svgHeight = 0;
    var svgWidth = 0;
    if (exportType == null || exportType == undefined) {
        exportType = 'EXCEL';
    }
    if (SourceType == "Rack" || SourceType == "POP" || SourceType == "MPOD" || SourceType == "Floor" || SourceType == "UNIT" || SourceType == "Cabinet") {
        if (SourceType == "Rack") {
            SourceId = $('#rackTabContext').find('.rackActive').attr('data-db-id');
            targetElem = $('.rackContext');
            //targetElem = $('.rackViewContext');
            svgHeight = $(rackViewContext).height();
            svgWidth = $(rackViewContext).width();
        }
        else {
            SourceId = $('#ispPOPId').val();
            targetElem = $('.roomContext');
            svgHeight = $(roomViewContext).height();
            svgWidth = $(roomViewContext).width();
        }
        document.body.style.zoom = GetZoomPercentage(svgWidth);
        var elements = targetElem.closest('svg g').find('svg').map(function () {
            var svg = $(this);
            var canvas = $('<canvas></canvas>').css({ position: 'absolute', fill: 'none', left: svg.css('left'), top: svg.css('top') });
            svg.replaceWith(canvas);
            //  Get the raw SVG string and curate it
            var content = svg.wrap('<p></p>').parent().html();
            svg.unwrap();
            canvg(canvas[0], content);
            return {
                svg: svg,
                canvas: canvas
            };
        });

        html2canvas($(targetElem)[0], {
            allowTaint: true, useCORS: true, logging: false, width: svgWidth, height: svgHeight, windowHeight: svgHeight + window.innerHeight + window.outerHeight
        }).then(function (canvas) {
            // Put the SVGs back in place
            elements.each(function () {
                this.canvas.replaceWith(this.svg);
            });
            var base64encodedstring = canvas.toDataURL("image/png", 1);
            var canvasimgdata = base64encodedstring.replace(/^data:image\/(png|jpg);base64,/, "");
            //Save Image in filesystem and get fullpath of image
            ajaxReq('ISP/SaveCaptureImage', { 'imgdata': canvasimgdata }, true, function (resp) {
                if (resp != null && resp.status == true) {
                     //Get Excel file with Image
                    window.location = appRoot + 'ISP/ExportRoomViewConnections?is_source_connected='
                        + IsConnectionFilter + "&source_type=" + SourceType + "&source_id=" + SourceId +
                        '&image_path=' + resp.file + '&exportType=' + exportType +
                        '&parent_type=' + parentType + '&parent_id=' + parentId;

                    hideProgress();
                }
            }, false, true, false);
        });

    }

    else if (SourceType == "Equipment") {
       // SourceId = $('#EntityExport').attr('data-system-id');
        SourceId = $("#EquipmentID").val();
        $('.FinalResult').append($('[data-element-id=' + SourceId+']').clone()).html();
        targetElem = $('.FinalResult');
      
         

        var parentType = $('#ParentType').val();
        var parentId = $('#ParentID').val();
        var svgImage = $('.FinalResult').children('svg')[0];
        var serializer = new XMLSerializer();
        var str = serializer.serializeToString(svgImage);
        var canvas = $('<canvas></canvas>').css({ position: 'absolute', fill: 'none', left: 'auto', top: 'auto' });
        canvas.appendTo('body');
        canvg(canvas.get(0), str);
        eqphtml2canvas($(canvas), {
            onrendered: function (canvas) {
                var base64encodedstring = canvas.toDataURL("image/png", 1);
                var canvasimgdata = base64encodedstring.replace(/^data:image\/(png|jpg);base64,/, "");
                $("canvas").remove();
                ajaxReq('ISP/SaveCaptureImage', { 'imgdata': canvasimgdata }, true, function (resp) {
                    if (resp != null && resp.status == true) {
                         
                        console.log(resp);                    //Get Excel file with Image
                        window.location = appRoot + 'ISP/ExportRoomViewConnections?is_source_connected='
                            + IsConnectionFilter + "&source_type=" + SourceType + "&source_id=" + SourceId +
                            '&image_path=' + resp.file + '&exportType=' + exportType +
                        '&parent_type=' + parentType + '&parent_id=' + parentId;;

                        hideProgress();
                    }
                }, false, true, false);

            }
        });
    }
    document.body.style.zoom = "100%";
}

function GetZoomPercentage(svgSize) {
     
    var windowWidth = $(window).width();
    var percentage = (windowWidth / (svgSize+270)) * 100;
    if (percentage > 100)
        return 100 + "%";
    return percentage - 10 + "%";
}
