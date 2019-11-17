import { Room } from './room';
import { SensorHistory } from './sensor-history';
import { DeviceDetails } from './device-details';

export interface RoomDetails {
    id: string,
    name: string,
    type: string,
    sensorsHistory: SensorHistory[],
    devices: DeviceDetails[]
}