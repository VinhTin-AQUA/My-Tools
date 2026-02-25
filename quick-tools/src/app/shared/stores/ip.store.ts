import { signalStore, withState, withMethods, patchState } from '@ngrx/signals';
import { IpAddressInfo } from '../../core/models/payload-commands/ip.payload';

const initialState: IpAddressInfo = {
    status: '-',
    country: '-',
    countryCode: '-',
    region: '-',
    regionName: '-',
    city: '-',
    zip: '-',
    lat: 0,
    lon: 0,
    timezone: '-',
    isp: '-',
    org: '-',
    as: '-',
    query: '-',
};

export const IpStore = signalStore(
    {
        providedIn: 'root',
    },
    withState(initialState),
    withMethods((store) => {
        function update(model: IpAddressInfo) {
            patchState(store, (currentState) => ({
                status: model.status,
                country: model.country,
                countryCode: model.countryCode,
                region: model.region,
                regionName: model.regionName,
                city: model.city,
                zip: model.zip,
                lat: model.lat,
                lon: model.lon,
                timezone: model.timezone,
                isp: model.isp,
                org: model.org,
                as: model.as,
                query: model.query,
            }));
        }

        return {
            update,
        };
    })
);
