import { Component, OnInit } from '@angular/core';
import { MatDialog, MatDialogConfig } from '@angular/material/dialog';
import { MatTableDataSource } from '@angular/material/table';
import { Router } from '@angular/router';
import { NgxUiLoaderComponent, NgxUiLoaderService } from 'ngx-ui-loader';
import { CategoryService } from 'src/app/services/category.service';
import { SnackbarService } from 'src/app/services/snackbar.service';
import { ThemeService } from 'src/app/services/theme.service';
import { GlobalConstants } from 'src/app/shared/global-constants';
import { CategoryComponent } from '../dialog/category/category.component';

@Component({
  selector: 'app-manage-category',
  templateUrl: './manage-category.component.html',
  styleUrls: ['./manage-category.component.scss']
})
export class ManageCategoryComponent implements OnInit{
  displayColumns : string[] = ['name', 'edit'];
  dataSource: any;
  responseMessage:any;

  constructor(private categoryService:CategoryService,
    private ngxService:NgxUiLoaderService,
    private dialog: MatDialog,
    private snackbarService:SnackbarService,
    private router: Router,
    public themeService:ThemeService) { }

    ngOnInit(): void {
      this.ngxService.start();
      this.tableData();
    }

    tableData(){
      this.categoryService.getAllCategory().subscribe((response:any) => {
        this.ngxService.stop();
        this.dataSource = new MatTableDataSource(response);
      }, (error:any) => {
        if(error.error?.message){
          this.responseMessage = error.error?.message;
        } else {
          this.responseMessage = GlobalConstants.genericError
        }
        this.snackbarService.openSnackBar(this.responseMessage);
      })
    }

    applyFilter(event:Event){
      const filterValue = (event.target as HTMLInputElement).value;
      this.dataSource.filter = filterValue.trim().toLowerCase();
    }

    handleAddAction(){
      debugger;
      const dialogConfig = new MatDialogConfig();
      dialogConfig.data = {
        action: 'Add'
      };
      dialogConfig.width = "850px";
      const dialogRef = this.dialog.open(CategoryComponent, dialogConfig);
      this.router.events.subscribe(() => {
        dialogRef.close();
      });
      const res = dialogRef.componentInstance.onAddCategory.subscribe(
        (response:any) => {
          this.tableData();
        }
      )
    }

    handleEditAction(values:any){
      const dialogConfig = new MatDialogConfig();
      dialogConfig.data = {
        action: 'Edit',
        data: values
      };
      dialogConfig.width = "850px";
      const dialogRef = this.dialog.open(CategoryComponent, dialogConfig);
      this.router.events.subscribe(() => {
        dialogRef.close();
      });
      const res = dialogRef.componentInstance.onAddCategory.subscribe(
        (response:any) => {
          this.tableData();
        }
      )
      
    }
}
