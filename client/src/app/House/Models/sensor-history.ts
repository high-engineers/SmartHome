import { SensorHistoryEntry } from './sensor-history-entry';

export interface SensorHistory {
    type: string,
    historyEntries: SensorHistoryEntry[]
}
