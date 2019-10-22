import { Device } from './device';

export interface Room {
    id: string,
    name: string,
    fontAwesome: string,
    temperature: number,
    humidity: number,
    devices: Device[]
}
