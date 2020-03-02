using System;
using Android.App;
using Android.OS;
using Android.Widget;
using Android.Util;

namespace JhpDataSystem
{
    public class DatePickerFragment : DialogFragment,
                                      DatePickerDialog.IOnDateSetListener
    {
        //https://developer.xamarin.com/guides/android/user_interface/date_picker/
        public static readonly string TAG = "X:" + typeof(DatePickerFragment).Name.ToUpper();
        Action<DateTime> _dateSelectedHandler = delegate { };
        public static DatePickerFragment NewInstance(Action<DateTime> onDateSelected)
        {
            var frg = new DatePickerFragment()
            {
                _dateSelectedHandler = onDateSelected
            };
            //frg.Dialog.add
            return frg;
        }

        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            DateTime currently = DateTime.Now;
            DatePickerDialog dialog =
                new DatePickerDialog(Activity,
                                                           this,
                                                           currently.Year,
                                                           currently.Month - 1,
                                                           currently.Day);
            dialog.SetButton3(new Java.Lang.String("Not Specified"), (s, args) => { setNoDateSetClicked(); });
            return dialog;
        }

        void setNoDateSetClicked(){
            _dateSelectedHandler(new DateTime(1900, 01, 01));
        }

        public void OnDateSet(DatePicker view, int year, int monthOfYear, int dayOfMonth)
        {
            // Note: monthOfYear is a value between 0 and 11, not 1 and 12!
            DateTime selectedDate = new DateTime(year, monthOfYear + 1, dayOfMonth);
            Log.Debug(TAG, selectedDate.ToLongDateString());
            _dateSelectedHandler(selectedDate);
        }
    }

}