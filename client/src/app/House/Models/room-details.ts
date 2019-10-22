import { Room } from './room';
import { SensorHistory } from './sensor-history';

export interface RoomDetails extends Room {
    sensorsHistory: SensorHistory[]
}
