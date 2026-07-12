export function ConvertToQueryString(obj: any) {
    /* obj exxample */
    // filters = {
    //     transIdOrpayment: '',
    //     label: false,
    //     noLabel: false,
    // };
    let query = "";
    const keys = Object.keys(obj);

    keys.forEach((key) => {
        query += `${key}=${obj[key]}&`
    });

    return query;
}
