PASSWORDS_AMOUNT = 100000

## Put the file here, or point it the existing location.
CORRUPTED_FILE_PATH = "C:\\pwned-passwords-sha1-ordered-by-count-v5.txt"

## Will create the files in the same directory as the repository.
RESULT_FILE_PATH = "..\\..\\corrupted-passwords-small.txt"
RESULT_FREQ_FILE_PATH = "..\\..\\corrupted-passwords-small-with-freq.txt"

with open (RESULT_FREQ_FILE_PATH, "a+") as file_write_freq:
    with open(RESULT_FILE_PATH, "a+") as file_write:
        with open(CORRUPTED_FILE_PATH, "r") as file_read:
            data = file_read.readline()
            line = data.split(":")[0] + "\n"
            
            count = 1

            while line:
                file_write_freq.write(data)
                file_write.write(line)

                data = file_read.readline()
                line = data.split(":")[0] + "\n"
                
                count += 1

                if (count == PASSWORDS_AMOUNT):
                    break
