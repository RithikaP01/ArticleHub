import { Component, OnInit } from '@angular/core';
import { MatDialog, MatDialogConfig } from '@angular/material/dialog';
import { MatTableDataSource } from '@angular/material/table';
import { Router } from '@angular/router';
import { NgxUiLoaderService } from 'ngx-ui-loader';
import { AppUserService } from 'src/app/services/app-user.service';
import { SnackbarService } from 'src/app/services/snackbar.service';
import { ThemeService } from 'src/app/services/theme.service';
import { GlobalConstants } from 'src/app/shared/global-constants';
import { UsersComponent } from '../dialog/users/users.component';

@Component({
  selector: 'app-manage-users',
  templateUrl: './manage-users.component.html',
  styleUrls: ['./manage-users.component.scss']
})
export class ManageUsersComponent implements OnInit {
  displayColumns: string[] = ['name', 'email', 'status', 'edit'];
  dataSource: any;
  responseMessage:any;

  constructor(private ngxService: NgxUiLoaderService,
    private dialog:MatDialog,
    private snackbarService:SnackbarService,
    private router:Router,
    private appUserService:AppUserService,
    public themeService:ThemeService
  ) {}

  ngOnInit() : void {
    this.ngxService.start();
    this.tableData();
  }

  tableData(){
    debugger;
    this.appUserService.getAllAppuser().subscribe((response:any) => {
      this.ngxService.stop();
      this.dataSource = new MatTableDataSource(response);
    }, (error:any) => {
      this.ngxService.stop()
      console.log(error);
      if(error.error?.message){
        this.responseMessage = error.error?.message;
      }
      else {
        this.responseMessage = GlobalConstants.genericError;
      }
      this.snackbarService.openSnackBar(this.responseMessage);
    })
  }

  applyFilter(event:any){
    const filterValue = (event.target as HTMLInputElement).value;
    this.dataSource.filter = filterValue.trim().toLowerCase();
  }

  handleAddAction() {
    debugger;
    const dialogConfig = new MatDialogConfig();
    dialogConfig.data={
      action: 'Add'
    };
    
    dialogConfig.width="850px";
    const dialogRef= this.dialog.open(UsersComponent, dialogConfig);
    this.router.events.subscribe(()=> {
      dialogRef.close();
    });
    const res = dialogRef.componentInstance.onAddUser.subscribe(
      (response:any) => {
        this.tableData();
      }
    )
  }

  handleEditAction(values:any) {
    const dialogConfig = new MatDialogConfig();
    dialogConfig.data={
      action: 'Update',
      data: values
    };
    
    dialogConfig.width="850px";
    const dialogRef= this.dialog.open(UsersComponent, dialogConfig);
    this.router.events.subscribe(()=> {
      dialogRef.close();
    });
    const res = dialogRef.componentInstance.onEditUser.subscribe(
      (response) => {
        this.tableData();
      }
    )
  }

  onChange(status:any, id:any){
    this.ngxService.start();
    var data = {
      id: id,
      status: status.toString()
    }
    this.appUserService.updateUserStatus(data).subscribe((response:any) => {
      this.ngxService.stop();
      this.responseMessage = response?.message;
      this.snackbarService.openSnackBar(this.responseMessage);
      this.tableData()
    }, (error:any) => {
      this.ngxService.stop()
      console.log(error);
      if(error.error?.message){
        this.responseMessage = error.error?.message;
      }
      else {
        this.responseMessage = GlobalConstants.genericError;
      }
      this.snackbarService.openSnackBar(this.responseMessage);
    })
  }

}
