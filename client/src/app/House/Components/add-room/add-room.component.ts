import { Component, OnInit } from '@angular/core';
import { RoomsService } from '../../Services/rooms.service';
import { NewRoom } from '../../Models/new-room';
import { OnDestroy } from '@angular/core';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/internal/operators/takeUntil';
import { RoomDetails } from '../../Models/room-details';

@Component({
  selector: 'app-add-room',
  templateUrl: './add-room.component.html',
  styleUrls: ['./add-room.component.scss']
})
export class AddRoomComponent implements OnInit, OnDestroy {

  public name: string;
  public type: string;
  private newRoom: NewRoom;
  private _unsubscribe$: Subject<void> = new Subject<void>();
  private roomDetails: RoomDetails;

  constructor(private roomsService: RoomsService) { }

  ngOnInit() {
  }

  addRoom = function(): void{
    this.newRoom = {
      name: this.name,
      type: this.type
    };
    
    this.roomsService.addRoom(this.newRoom)
      .pipe(takeUntil(this._unsubscribe$))
        .subscribe(x => this.roomDetails = x);
  }

  ngOnDestroy(): void {
    this._unsubscribe$.next();
    this._unsubscribe$.complete();
  }

}
