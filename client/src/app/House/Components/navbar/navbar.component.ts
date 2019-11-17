import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Room } from '../../Models/room';
import { RoomsService } from '../../Services/rooms.service';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.scss']
})
export class NavbarComponent implements OnInit {

  public test: string;
  public rooms: Room[];

  constructor(
    private router: Router,
    private roomsService: RoomsService
  ) { }

  ngOnInit() {
    this.getRooms();
  }

  getRooms(): void {
    this.roomsService.getRooms()
      .subscribe(rooms => {
        this.rooms = rooms;
        this.showDetails(this.rooms[0].id);
      });
  }

  showDetails = function (id: string) {
    this.router.navigateByUrl('/rooms/' + id);
  };
}
