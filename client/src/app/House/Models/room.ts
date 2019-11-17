import { Device } from './device';

export interface Room {
    id: string,
    name: string,
    type: string,
    temperature: number,
    humidity: number,
    devices: Device[]
}